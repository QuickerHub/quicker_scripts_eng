// 基本回调测试
function runBasicTest() {
    log('开始测试Chrome API回调...');
    
    // 测试tabs.query
    chrome.tabs.query({}, function(tabs) {
        log('查询到 ' + tabs.length + ' 个标签页');
        
        for(var i = 0; i < tabs.length; i++) {
            var tab = tabs[i];
            log('标签页 #' + tab.id + ': ' + tab.title + ' (' + tab.url + ')');
        }
    });
    
    log('tabs.query已调用，等待回调...');
    return 'Basic test started';
}

// 嵌套回调测试
function runNestedTest() {
    log('开始测试嵌套回调...');
    
    // 查询标签页，然后创建新标签页，然后获取新标签页
    chrome.tabs.query({}, function(tabs) {
        log('查询到 ' + tabs.length + ' 个标签页');
        
        // 创建新标签页
        chrome.tabs.create({ url: 'https://www.example.org' }, function(newTab) {
            log('创建了新标签页: #' + newTab.id + ' - ' + newTab.url);
            
            // 获取新创建的标签页
            chrome.tabs.get(newTab.id, function(tab) {
                log('获取标签页: #' + tab.id + ' - ' + tab.title);
                log('嵌套回调测试完成');
            });
        });
    });
    
    log('嵌套回调测试已完成');
    return 'Nested test started';
}

// 多API测试
function runMultiApiTest() {
    log('开始测试更多Chrome API...');
    
    // 测试browsingData API
    chrome.browsingData.removeHistory({since: 0}, function(result) {
        log('清除浏览历史结果: ' + JSON.stringify(result));
        
        // 测试cookies API
        chrome.cookies.getAll({domain: 'example.com'}, function(cookies) {
            log('获取cookies: ' + JSON.stringify(cookies));
            
            // 测试downloads API
            chrome.downloads.search({limit: 5}, function(downloads) {
                log('搜索下载项: ' + JSON.stringify(downloads));
                
                // 测试history API
                chrome.history.search({text: '', maxResults: 5}, function(historyItems) {
                    log('搜索历史记录: ' + JSON.stringify(historyItems));
                    
                    // 测试tts API
                    chrome.tts.getVoices(function(voices) {
                        log('获取TTS语音: ' + JSON.stringify(voices));
                        log('更多API测试完成');
                    });
                });
            });
        });
    });
    
    log('更多API测试已启动...');
    return 'Multi-API test started';
} 