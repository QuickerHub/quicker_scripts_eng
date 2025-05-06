/**
 * Script Parser Engine for converting Manifest V2 background scripts to Manifest V3 API calls
 */

class ScriptParser {
    constructor() {
        // API namespace mapping
        this.apiNamespaces = {
            'chrome.tabs': 'api_tabs',
            'chrome.windows': 'api_windows',
            'chrome.storage': 'api_storage',
            'chrome.runtime': 'api_runtime',
            'chrome.cookies': 'api_cookies',
            'chrome.history': 'api_history',
            'chrome.downloads': 'api_downloads',
            'chrome.bookmarks': 'api_bookmarks',
            'chrome.webNavigation': 'api_webNavigation',
            'chrome.webRequest': 'api_webRequest',
            'chrome.scripting': 'api_scripting',
            'chrome.declarativeNetRequest': 'api_declarativeNetRequest'
        };
        // Babel functions will be injected
        this.babelParser = null;
        this.babelTraverse = null;
        this.babelTypes = null;
        this.babelGenerator = null;
    }

    // Method to inject Babel dependencies
    initializeBabel(parser, traverse, types, generator) {
        this.babelParser = parser;
        this.babelTraverse = traverse;
        this.babelTypes = types;
        this.babelGenerator = generator;
        
        if (!this.babelParser || !this.babelTraverse || !this.babelTypes || !this.babelGenerator) {
            throw new Error("Babel dependencies not initialized correctly.");
        }
    }

    /**
     * Parse and convert Chrome API calls to api_* format
     * @param {string} script - The original script
     * @returns {string} - The converted script
     */
    parse(script) {
        if (!this.babelParser) {
            throw new Error("Babel parser not initialized. Call initializeBabel first.");
        }
        // Parse the script into AST using the injected parser
        const ast = this.babelParser.parse(script, {
            sourceType: 'script', // Changed from 'module' based on previous successful attempt
            plugins: [] // Removed jsx/typescript plugins
        });

        // Transform the AST
        this.transformAST(ast);

        // Generate code from the transformed AST using the injected generator
        const output = this.babelGenerator.default(ast, {
            retainLines: true,
            comments: true,
            compact: false
        });

        return output.code;
    }

    /**
     * Transform the AST to convert Chrome API calls
     * @param {t.Node} ast - The AST to transform
     */
    transformAST(ast) {
        if (!this.babelTraverse || !this.babelTypes) {
            throw new Error("Babel traverse/types not initialized. Call initializeBabel first.");
        }
        const self = this; // Capture this context
        const t = this.babelTypes; // Use injected types
        
        this.babelTraverse.default(ast, { // Use injected traverse
            // Handle Chrome API calls
            CallExpression(path) {
                const { node } = path;
                
                // Check if this is a Chrome API call
                if (t.isMemberExpression(node.callee) && 
                    t.isMemberExpression(node.callee.object) &&
                    t.isIdentifier(node.callee.object.object) &&
                    node.callee.object.object.name === 'chrome') {
                    
                    const namespace = node.callee.object.property.name;
                    const method = node.callee.property.name;
                    const fullNamespace = `chrome.${namespace}`;
                    
                    if (self.apiNamespaces[fullNamespace]) {
                        // Convert to api_* format
                        const newMethodName = `${self.apiNamespaces[fullNamespace]}_${method}`;
                        
                        // Create new call expression
                        const newCallee = t.identifier(newMethodName);
                        
                        // Handle callbacks
                        if (node.arguments.length > 0 && 
                            (t.isFunctionExpression(node.arguments[node.arguments.length - 1]) ||
                             t.isArrowFunctionExpression(node.arguments[node.arguments.length - 1]))) {
                            
                            const callback = node.arguments.pop();
                            const params = node.arguments;
                            
                            // Create async IIFE
                            const asyncIIFE = t.callExpression(
                                t.arrowFunctionExpression(
                                    [],
                                    t.blockStatement([
                                        t.variableDeclaration('const', [
                                            t.variableDeclarator(
                                                t.identifier('result'),
                                                t.awaitExpression(
                                                    t.callExpression(newCallee, params)
                                                )
                                            )
                                        ]),
                                        t.expressionStatement(
                                            t.callExpression(callback, [t.identifier('result')])
                                        )
                                    ])
                                ),
                                []
                            );
                            
                            path.replaceWith(asyncIIFE);
                        } else {
                            // Simple API call without callback
                            path.replaceWith(t.callExpression(newCallee, node.arguments));
                        }
                    }
                }
            },

            // Handle event listeners
            MemberExpression(path) {
                const { node } = path;
                
                if (t.isMemberExpression(node.object) &&
                    t.isMemberExpression(node.object.object) &&
                    t.isIdentifier(node.object.object.object) &&
                    node.object.object.object.name === 'chrome' &&
                    node.property.name === 'addListener') {
                    
                    const namespace = node.object.object.property.name;
                    const event = node.object.property.name.replace('on', '');
                    const fullNamespace = `chrome.${namespace}`;
                    
                    if (self.apiNamespaces[fullNamespace]) {
                        const eventName = `${self.apiNamespaces[fullNamespace]}_on${event}`;
                        
                        // Replace with message listener
                        const parent = path.parentPath;
                        if (t.isCallExpression(parent.node)) {
                            const callback = parent.node.arguments[0];
                            
                            const messageListener = t.callExpression(
                                t.memberExpression(
                                    t.memberExpression(
                                        t.identifier('chrome'),
                                        t.identifier('runtime')
                                    ),
                                    t.identifier('onMessage')
                                ),
                                [
                                    t.arrowFunctionExpression(
                                        [t.identifier('message'), t.identifier('sender'), t.identifier('sendResponse')],
                                        t.blockStatement([
                                            t.ifStatement(
                                                t.binaryExpression(
                                                    '===',
                                                    t.memberExpression(t.identifier('message'), t.identifier('type')),
                                                    t.stringLiteral(eventName)
                                                ),
                                                t.blockStatement([
                                                    t.expressionStatement(
                                                        t.callExpression(
                                                            t.arrowFunctionExpression(
                                                                [],
                                                                t.blockStatement([
                                                                    t.variableDeclaration('const', [
                                                                        t.variableDeclarator(
                                                                            t.identifier('result'),
                                                                            t.awaitExpression(
                                                                                t.callExpression(
                                                                                    callback,
                                                                                    [t.memberExpression(t.identifier('message'), t.identifier('data')), t.identifier('sender')]
                                                                                )
                                                                            )
                                                                        )
                                                                    ]),
                                                                    t.expressionStatement(
                                                                        t.callExpression(t.identifier('sendResponse'), [t.identifier('result')])
                                                                    )
                                                                ])
                                                            ),
                                                            []
                                                        )
                                                    ),
                                                    t.returnStatement(t.booleanLiteral(true))
                                                ])
                                            )
                                        ])
                                    )
                                ]
                            );
                            
                            parent.replaceWith(messageListener);
                        }
                    }
                }
            }
        });
    }

    /**
     * Generate a command handler for a specific API call
     * @param {string} apiCall - The API call to handle
     * @returns {string} - The command handler code
     */
    generateCommandHandler(apiCall) {
        const [namespace, method] = apiCall.split('_');
        return `
async function ${apiCall}(params, msg) {
    try {
        const result = await chrome.${namespace}.${method}(params);
        return result;
    } catch (error) {
        console.error('Error executing ${apiCall}:', error);
        throw error;
    }
}`;
    }
}

// Export the parser
export default ScriptParser; 