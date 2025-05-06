// 创建chrome对象
if (typeof chrome === 'undefined') {
    chrome = {};
}

// API调用函数，处理回调
function _callApi(apiName, params, callback) {
    try {
        // 确保参数是对象
        params = params || {};
        
        // 调用.NET原生API
        var result = _nativeApiCall(apiName, params);
        
        // 处理回调
        if (typeof callback === 'function') {
            // 直接调用回调函数
            try {
                callback(result);
                console.log('[JS] 已执行回调');
            } catch (err) {
                console.log('[JS] 回调执行错误: ' + err);
            }
        }
        
        return result;
    } catch (err) {
        console.log('[JS] API调用错误: ' + err);
        return { error: err.toString() };
    }
}

// 定义tabs API
chrome.tabs = {
    // 基本方法
    query: function(queryInfo, callback) { return _callApi('api_tabs_query', queryInfo || {}, callback); },
    get: function(tabId, callback) { return _callApi('api_tabs_get', { tabId: tabId }, callback); },
    create: function(createProperties, callback) { return _callApi('api_tabs_create', createProperties || {}, callback); },
    
    // 扩展方法
    captureVisibleTab: function(windowId, options, callback) { return _callApi('api_tabs_captureVisibleTab', {windowId: windowId, options: options}, callback); },
    detectLanguage: function(tabId, callback) { return _callApi('api_tabs_detectLanguage', {tabId: tabId}, callback); },
    discard: function(tabId, callback) { return _callApi('api_tabs_discard', {tabId: tabId}, callback); },
    duplicate: function(tabId, callback) { return _callApi('api_tabs_duplicate', {tabId: tabId}, callback); },
    getCurrent: function(callback) { return _callApi('api_tabs_getCurrent', {}, callback); },
    getZoom: function(tabId, callback) { return _callApi('api_tabs_getZoom', {tabId: tabId}, callback); },
    getZoomSettings: function(tabId, callback) { return _callApi('api_tabs_getZoomSettings', {tabId: tabId}, callback); },
    goBack: function(tabId, callback) { return _callApi('api_tabs_goBack', {tabId: tabId}, callback); },
    goForward: function(tabId, callback) { return _callApi('api_tabs_goForward', {tabId: tabId}, callback); },
    group: function(options, callback) { return _callApi('api_tabs_group', options, callback); },
    highlight: function(highlightInfo, callback) { return _callApi('api_tabs_highlight', highlightInfo, callback); },
    move: function(tabIds, moveProperties, callback) { return _callApi('api_tabs_move', {tabIds: tabIds, moveProperties: moveProperties}, callback); },
    reload: function(tabId, reloadProperties, callback) { return _callApi('api_tabs_reload', {tabId: tabId, reloadProperties: reloadProperties}, callback); },
    remove: function(tabIds, callback) { return _callApi('api_tabs_remove', {tabIds: tabIds}, callback); },
    sendMessage: function(tabId, message, options, callback) { return _callApi('api_tabs_sendMessage', {tabId: tabId, message: message, options: options}, callback); },
    setZoom: function(tabId, zoomFactor, callback) { return _callApi('api_tabs_setZoom', {tabId: tabId, zoomFactor: zoomFactor}, callback); },
    setZoomSettings: function(tabId, zoomSettings, callback) { return _callApi('api_tabs_setZoomSettings', {tabId: tabId, zoomSettings: zoomSettings}, callback); },
    toggleMuteState: function(tabId, callback) { return _callApi('api_tabs_toggleMuteState', {tabId: tabId}, callback); },
    ungroup: function(tabIds, callback) { return _callApi('api_tabs_ungroup', {tabIds: tabIds}, callback); },
    update: function(tabId, updateProperties, callback) { return _callApi('api_tabs_update', {tabId: tabId, updateProperties: updateProperties}, callback); }
};

// windows API
chrome.windows = {
    create: function(createData, callback) { return _callApi('api_windows_create', createData, callback); },
    get: function(windowId, getInfo, callback) { return _callApi('api_windows_get', {windowId: windowId, getInfo: getInfo}, callback); },
    getAll: function(getInfo, callback) { return _callApi('api_windows_getAll', getInfo, callback); },
    getCurrent: function(getInfo, callback) { return _callApi('api_windows_getCurrent', getInfo, callback); },
    getLastFocused: function(getInfo, callback) { return _callApi('api_windows_getLastFocused', getInfo, callback); },
    remove: function(windowId, callback) { return _callApi('api_windows_remove', {windowId: windowId}, callback); },
    update: function(windowId, updateInfo, callback) { return _callApi('api_windows_update', {windowId: windowId, updateInfo: updateInfo}, callback); }
};

// bookmarks API
chrome.bookmarks = {
    create: function(bookmark, callback) { return _callApi('api_bookmarks_create', bookmark, callback); },
    get: function(idOrIdList, callback) { return _callApi('api_bookmarks_get', {id: idOrIdList}, callback); },
    getChildren: function(id, callback) { return _callApi('api_bookmarks_getChildren', {id: id}, callback); },
    getRecent: function(numberOfItems, callback) { return _callApi('api_bookmarks_getRecent', {numberOfItems: numberOfItems}, callback); },
    getTree: function(callback) { return _callApi('api_bookmarks_getTree', {}, callback); },
    move: function(id, destination, callback) { return _callApi('api_bookmarks_move', {id: id, destination: destination}, callback); },
    remove: function(id, callback) { return _callApi('api_bookmarks_remove', {id: id}, callback); },
    removeTree: function(id, callback) { return _callApi('api_bookmarks_removeTree', {id: id}, callback); },
    search: function(query, callback) { return _callApi('api_bookmarks_search', query, callback); },
    update: function(id, changes, callback) { return _callApi('api_bookmarks_update', {id: id, changes: changes}, callback); }
};

// browsingData API
chrome.browsingData = {
    remove: function(options, dataToRemove, callback) { return _callApi('api_browsingData_remove', {options: options, dataToRemove: dataToRemove}, callback); },
    removeAppcache: function(options, callback) { return _callApi('api_browsingData_removeAppcache', options, callback); },
    removeCache: function(options, callback) { return _callApi('api_browsingData_removeCache', options, callback); },
    removeCookies: function(options, callback) { return _callApi('api_browsingData_removeCookies', options, callback); },
    removeDownloads: function(options, callback) { return _callApi('api_browsingData_removeDownloads', options, callback); },
    removeFileSystems: function(options, callback) { return _callApi('api_browsingData_removeFileSystems', options, callback); },
    removeFormData: function(options, callback) { return _callApi('api_browsingData_removeFormData', options, callback); },
    removeHistory: function(options, callback) { return _callApi('api_browsingData_removeHistory', options, callback); },
    removeIndexedDB: function(options, callback) { return _callApi('api_browsingData_removeIndexedDB', options, callback); },
    removeLocalStorage: function(options, callback) { return _callApi('api_browsingData_removeLocalStorage', options, callback); },
    removePasswords: function(options, callback) { return _callApi('api_browsingData_removePasswords', options, callback); },
    removePluginData: function(options, callback) { return _callApi('api_browsingData_removePluginData', options, callback); },
    removeServiceWorkers: function(options, callback) { return _callApi('api_browsingData_removeServiceWorkers', options, callback); },
    removeWebSQL: function(options, callback) { return _callApi('api_browsingData_removeWebSQL', options, callback); },
    settings: function(callback) { return _callApi('api_browsingData_settings', {}, callback); }
};

// cookies API
chrome.cookies = {
    get: function(details, callback) { return _callApi('api_cookies_get', details, callback); },
    getAll: function(details, callback) { return _callApi('api_cookies_getAll', details, callback); },
    set: function(details, callback) { return _callApi('api_cookies_set', details, callback); },
    remove: function(details, callback) { return _callApi('api_cookies_remove', details, callback); },
    getAllCookieStores: function(callback) { return _callApi('api_cookies_getAllCookieStores', {}, callback); }
};

// debugger API
chrome.debugger = {
    attach: function(target, requiredVersion, callback) { return _callApi('api_debugger_attach', {target: target, requiredVersion: requiredVersion}, callback); },
    detach: function(target, callback) { return _callApi('api_debugger_detach', {target: target}, callback); },
    sendCommand: function(target, method, commandParams, callback) { return _callApi('api_debugger_sendCommand', {target: target, method: method, commandParams: commandParams}, callback); },
    getTargets: function(callback) { return _callApi('api_debugger_getTargets', {}, callback); }
};

// downloads API
chrome.downloads = {
    download: function(options, callback) { return _callApi('api_downloads_download', options, callback); },
    search: function(query, callback) { return _callApi('api_downloads_search', query, callback); },
    pause: function(downloadId, callback) { return _callApi('api_downloads_pause', {downloadId: downloadId}, callback); },
    resume: function(downloadId, callback) { return _callApi('api_downloads_resume', {downloadId: downloadId}, callback); },
    cancel: function(downloadId, callback) { return _callApi('api_downloads_cancel', {downloadId: downloadId}, callback); },
    erase: function(query, callback) { return _callApi('api_downloads_erase', query, callback); },
    removeFile: function(downloadId, callback) { return _callApi('api_downloads_removeFile', {downloadId: downloadId}, callback); },
    open: function(downloadId, callback) { return _callApi('api_downloads_open', {downloadId: downloadId}, callback); },
    show: function(downloadId, callback) { return _callApi('api_downloads_show', {downloadId: downloadId}, callback); },
    showDefaultFolder: function(callback) { return _callApi('api_downloads_showDefaultFolder', {}, callback); },
    getFileIcon: function(downloadId, options, callback) { return _callApi('api_downloads_getFileIcon', {downloadId: downloadId, options: options}, callback); },
    setShelfEnabled: function(enabled) { return _callApi('api_downloads_setShelfEnabled', {enabled: enabled}); }
};

// history API
chrome.history = {
    search: function(query, callback) { return _callApi('api_history_search', query, callback); },
    getVisits: function(details, callback) { return _callApi('api_history_getVisits', details, callback); },
    addUrl: function(details, callback) { return _callApi('api_history_addUrl', details, callback); },
    deleteUrl: function(details, callback) { return _callApi('api_history_deleteUrl', details, callback); },
    deleteRange: function(range, callback) { return _callApi('api_history_deleteRange', range, callback); },
    deleteAll: function(callback) { return _callApi('api_history_deleteAll', {}, callback); }
};

// pageCapture API
chrome.pageCapture = {
    saveAsMHTML: function(details, callback) { return _callApi('api_pageCapture_saveAsMHTML', details, callback); }
};

// readingList API
chrome.readingList = {
    add: function(addOptions, callback) { return _callApi('api_readingList_add', addOptions, callback); },
    getEntries: function(options, callback) { return _callApi('api_readingList_getEntries', options, callback); },
    remove: function(entryId, callback) { return _callApi('api_readingList_remove', {entryId: entryId}, callback); },
    update: function(entryId, updateOptions, callback) { return _callApi('api_readingList_update', {entryId: entryId, updateOptions: updateOptions}, callback); }
};

// tabGroups API
chrome.tabGroups = {
    get: function(groupId, callback) { return _callApi('api_tabGroups_get', {groupId: groupId}, callback); },
    update: function(groupId, updateProperties, callback) { return _callApi('api_tabGroups_update', {groupId: groupId, updateProperties: updateProperties}, callback); },
    move: function(groupId, moveProperties, callback) { return _callApi('api_tabGroups_move', {groupId: groupId, moveProperties: moveProperties}, callback); },
    query: function(queryInfo, callback) { return _callApi('api_tabGroups_query', queryInfo, callback); }
};

// tts API
chrome.tts = {
    speak: function(utterance, options, callback) { return _callApi('api_tts_speak', {utterance: utterance, options: options}, callback); },
    stop: function() { return _callApi('api_tts_stop', {}); },
    pause: function() { return _callApi('api_tts_pause', {}); },
    resume: function() { return _callApi('api_tts_resume', {}); },
    isSpeaking: function(callback) { return _callApi('api_tts_isSpeaking', {}, callback); },
    getVoices: function(callback) { return _callApi('api_tts_getVoices', {}, callback); }
};

// 日志函数
if (typeof console === 'undefined') {
    console = { log: function(msg) { log(msg); } };
}

console.log('[JS] Chrome API 初始化完成'); 