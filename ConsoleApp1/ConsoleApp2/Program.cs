// Chrome API 桥接回调测试 - MVP版本

using ChromeController;
using log4net.Config;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // 初始化log4net
            XmlConfigurator.Configure(new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App.config")));

            Console.WriteLine("Chrome API 桥接回调测试 - MVP版本");
            Console.WriteLine("===================================");

            try
            {
                // 创建脚本引擎
                var engine = new ScriptEngineWrapper();

                // 创建简化版Chrome桥接
                var chrome = new ChromeBridge(engine);

                // 注入日志函数
                engine.InjectFunction("log", new Action<string>(message =>
                {
                    Console.WriteLine($"[JS日志] {message}");
                }));

                // 运行基本测试
                RunBasicTests(engine);

                // 运行Grouper类测试
                TestGrouperClass(engine);

                Console.WriteLine("\n测试完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"测试过程中出现异常: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        static void RunBasicTests(ScriptEngineWrapper engine)
        {
            Console.WriteLine("\n1. 执行基本回调测试:");

            // 执行一个简单的测试脚本
            string basicTest = @"
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
            ";

            engine.Execute(basicTest);

            // 不再需要等待异步回调，因为回调是同步执行的

            Console.WriteLine("\n2. 执行嵌套回调测试:");

            // 嵌套回调测试
            string nestedTest = @"
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
            ";

            engine.Execute(nestedTest);

            Console.WriteLine("\n3. 测试其他Chrome API:");

            // 测试更多API
            string moreApisTest = @"
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
            ";

            engine.Execute(moreApisTest);
        }

        static void TestGrouperClass(ScriptEngineWrapper engine)
        {
            Console.WriteLine("\n4. 测试Grouper类功能:");

            // 定义Grouper类
            string grouperClassDefinition =
"""
/**
 * 自定义URL类，主要用于获取hostname
 */
class URL {
    /**
     * 创建URL对象
     * @param {String} url URL字符串
     */
    constructor(url) {
        // 解析hostname
        let withoutProtocol = url;
        if (url.indexOf('://') > -1) {
            withoutProtocol = url.split('://')[1];
        }
        
        // 获取域名部分（忽略路径和其他组件）
        let domainPart = withoutProtocol.split('/')[0];
        
        // 移除端口号（如果有）
        if (domainPart.indexOf(':') > -1) {
            domainPart = domainPart.split(':')[0];
        }
        
        this.hostname = domainPart;
    }
}

class grouper {
    /**
     * 
     * @param {Object} d_map 
     */
    constructor(d_map) {
        this.d_map = d_map;
    }

    /**
     * Quicker子程序调用接口函数
     * @param {String} operation 
     * @param {String} type 
     */
    groupOperation(operation, type) {
        if (operation == "domain") {
            switch (type) {
                case "lastWindow": this.groupTabsInLastFocusedWindow(); break;
                case "activeTab": this.groupActiveTab(); break;
                case "all": this.groupAllTabs(); break;
            }
        }
    }

    /**对当前活动窗口中的标签页进行分组 */
    groupTabsInLastFocusedWindow() {
        log('调用: groupTabsInLastFocusedWindow()');
        chrome.tabs.query({ lastFocusedWindow: true }, tabs => {
            this.groupSomeTabs(tabs);
        })
    }

    /**对当前活动标签页进行分组(在当前窗口中) */
    groupActiveTab() {
        log('调用: groupActiveTab()');
        chrome.tabs.query({ lastFocusedWindow: true }, tabs => {
            var [activeTab] = tabs.filter(tab => tab.active);
            var name = new URL(activeTab.url).hostname;
            this.tabsToOneGroup(tabs.filter(tab => new URL(tab.url).hostname == name));
        })
    }
    /**
     * 对浏览器中所有标签进行分组
     */
    groupAllTabs() {
        log('调用: groupAllTabs()');
        chrome.tabs.query({}, tabs => this.groupSomeTabs(tabs));
    }

    /**
     * 通过一组id对标签页进行分组
     * @param {Array} tabIds 
     */
    groupSomeTabsById(tabIds) {
        log('调用: groupSomeTabsById()');
        chrome.tabs.query({}, tabs => {
            this.groupSomeTabs(tabs.filter(tab => tab.id in tabIds));
        })
    }

    /**
     * group 一组 tab
     * 结果可能是多个分组
     * @param {chrome.tabs.Tab[]} tabs
     */
    groupSomeTabs(tabs) {
        log('调用: groupSomeTabs()，标签页数量: ' + tabs.length);
        var dict = {};
        tabs.forEach(tab => {
            var hostname = new URL(tab.url).hostname;
            if (hostname in dict) {
                dict[hostname].push(tab);
            }
            else {
                dict[hostname] = [tab];
            }
        });
        log('分组结果: ' + JSON.stringify(Object.keys(dict)));
        Object.keys(dict).filter(key => dict[key].length > 1).map(x => dict[x]).forEach(tabs => {
            this.tabsToOneGroup(tabs);
        })
    }
    /**
     * 标签添加到同一个组并保留原来的组
     * @param {chrome.tabs.Tab[]} tabs 
     */
    tabsToOneGroup(tabs) {
        log('调用: tabsToOneGroup()，标签页数量: ' + tabs.length);
        var filterTabs = tabs.filter(x => x.groupId != -1);
        if (filterTabs.length > 0) {
            log('添加到现有分组: ' + filterTabs[0].groupId);
            chrome.tabs.group({
                tabIds: tabs.map(tab => tab.id),
                groupId: filterTabs[0].groupId
            });
        }
        else {
            //新建一个组,自动命名
            var group_title = this.GetGroupTitle(tabs[0].url);
            log('创建新分组，标题: ' + group_title);
            chrome.tabs.group({
                tabIds: tabs.map(tab => tab.id)
            }, groupId => {
                chrome.tabs.update(groupId, {
                    title: group_title
                });
            })
        }
    }

    /**
     * url获取可能的名称
     * @param {String} url 
     */
    GetGroupTitle(url) {
        // 移除协议部分 (http://, https://, etc.)
        var hostname = new URL(url).hostname;
        
        // 分割域名部分并提取主要名称
        var parts = hostname.split('.');
        var name = parts[0] == "www" && parts.length > 1 ? parts[1] : parts[0];
        
        log('基础方式提取hostname: ' + url + ' -> ' + hostname + ' -> ' + name);
        return name;
    }
}                
""";

            // 执行Grouper类测试
            string grouperTest = $@"
                {grouperClassDefinition}
                
                log('开始测试grouper类...');
                
                // 创建一个grouper实例
                var g = new grouper();
                
                // 测试1: 分组最后活动窗口的标签页
                log('测试1: 分组最后活动窗口的标签页');
                g.groupOperation('domain', 'lastWindow');
                
                // 测试2: 分组活动标签页
                log('测试2: 分组活动标签页');
                g.groupOperation('domain', 'activeTab');
                
                // 测试3: 分组所有标签页
                log('测试3: 分组所有标签页');
                g.groupOperation('domain', 'all');
                
                // 测试4: 测试GetGroupTitle方法
                log('测试4: 测试GetGroupTitle方法');
                var testUrls = [
                    'https://www.example.com/path',
                    'https://example.org/test',
                    'https://subdomain.example.net/page'
                ];
                
                testUrls.forEach(url => {{
                    var title = g.GetGroupTitle(url);
                    log('URL: ' + url + ' => 标题: ' + title);
                }});
                
                log('grouper类测试完成');
            ";

            engine.Execute(grouperTest);
        }
    }
}
