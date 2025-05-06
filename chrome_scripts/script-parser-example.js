import ScriptParser from './script-parser.js';

// Example script with Chrome API calls
const originalScript = `
// Example 1: Simple API call
chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
    console.log('Current tab:', tabs[0]);
});

// Example 2: API call with arrow function callback
chrome.storage.local.get(['settings'], (result) => {
    console.log('Settings:', result.settings);
});

// Example 3: API call with async/await
async function getCurrentTab() {
    const tabs = await chrome.tabs.query({active: true, currentWindow: true});
    return tabs[0];
}

// Example 4: Event listener
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    if (changeInfo.status === 'complete') {
        console.log('Tab updated:', tabId);
    }
});

// Example 5: Multiple API calls
chrome.storage.local.get(['config'], function(result) {
    if (result.config) {
        chrome.tabs.create({url: result.config.homepage});
    }
});
`;

// Create parser instance
const parser = new ScriptParser();

// Parse the script
const convertedScript = parser.parse(originalScript);

console.log('Original Script:');
console.log(originalScript);
console.log('\nConverted Script:');
console.log(convertedScript);

// Expected converted output
const expectedOutput = `
// Example 1: Simple API call
(async () => {
    const result = await api_tabs_query({active: true, currentWindow: true});
    console.log('Current tab:', result[0]);
})();

// Example 2: API call with arrow function callback
(async () => {
    const result = await api_storage_local_get(['settings']);
    console.log('Settings:', result.settings);
})();

// Example 3: API call with async/await
async function getCurrentTab() {
    const tabs = await api_tabs_query({active: true, currentWindow: true});
    return tabs[0];
}

// Example 4: Event listener
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.type === 'api_tabs_onUpdated') {
        (async () => {
            const result = await (tabId, changeInfo, tab) => {
                if (changeInfo.status === 'complete') {
                    console.log('Tab updated:', tabId);
                }
            }(message.data, sender);
            sendResponse(result);
        })();
        return true;
    }
});

// Example 5: Multiple API calls
(async () => {
    const result = await api_storage_local_get(['config']);
    if (result.config) {
        await api_tabs_create({url: result.config.homepage});
    }
})();
`;

console.log('\nExpected Output:');
console.log(expectedOutput); 