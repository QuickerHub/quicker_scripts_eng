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
    }
}
