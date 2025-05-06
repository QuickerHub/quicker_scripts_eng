import ScriptParser from './script-parser.js';

// Test case 1: Simple API call with callback
const testCase1 = `
chrome.tabs.query({active: true}, function(tabs) {
    console.log(tabs[0]);
});
`;

// Test case 2: Event listener
const testCase2 = `
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    console.log('Tab updated:', tabId);
});
`;

// Test case 3: Storage API with arrow function
const testCase3 = `
chrome.storage.local.get(['key'], (result) => {
    console.log(result.key);
});
`;

// Test case 4: Nested API calls
const testCase4 = `
chrome.tabs.query({active: true}, function(tabs) {
    chrome.storage.local.get(['config'], (result) => {
        console.log(tabs[0], result.config);
    });
});
`;

// Test case 5: Multiple event listeners
const testCase5 = `
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    console.log('Tab updated:', tabId);
});

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    console.log('Message received:', request);
    sendResponse({status: 'ok'});
});
`;

const parser = new ScriptParser();

function runTest(name, script) {
    console.log(`\n${name}:`);
    console.log('Original:', script);
    console.log('Converted:', parser.parse(script));
}

runTest('Test Case 1: Simple API call', testCase1);
runTest('Test Case 2: Event listener', testCase2);
runTest('Test Case 3: Storage API', testCase3);
runTest('Test Case 4: Nested API calls', testCase4);
runTest('Test Case 5: Multiple event listeners', testCase5); 