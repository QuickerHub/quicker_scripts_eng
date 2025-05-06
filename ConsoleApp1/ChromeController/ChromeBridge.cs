using System.Reflection;
using Newtonsoft.Json.Linq;
using Quicker.Utilities._3rd.Chrome;

namespace ChromeController;

/// <summary>
/// Chrome 浏览器 API 调用桥接类的最小化实现
/// 只实现了基本的tabs API和回调处理方式
/// </summary>
public class ChromeBridge
{
    private readonly ScriptEngineWrapper _scriptEngine;
    
    // 模拟数据存储 - 简单的标签页列表
    private static readonly List<JObject> _tabs = new List<JObject>
    {
        new JObject
        {
            ["id"] = 1,
            ["url"] = "https://www.example.com",
            ["title"] = "Example Site"
        },
        new JObject
        {
            ["id"] = 2,
            ["url"] = "https://www.google.com",
            ["title"] = "Google"
        }
    };

    /// <summary>
    /// 初始化Chrome桥接
    /// </summary>
    public ChromeBridge(ScriptEngineWrapper scriptEngine)
    {
        _scriptEngine = scriptEngine ?? throw new ArgumentNullException(nameof(scriptEngine));
        
        // 注入简单的API调用函数，返回结果由JavaScript端处理
        _scriptEngine.InjectFunction("_nativeApiCall", new Func<string, object, JToken>((apiName, parameters) => {
            try
            {
                Console.WriteLine($"[ChromeBridgeMVP] 收到API调用: {apiName}");
                return HandleApiCall(apiName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChromeBridgeMVP] API调用异常: {ex.Message}");
                return new JObject { ["error"] = ex.Message };
            }
        }));
        
        // 从嵌入资源加载Chrome API脚本
        LoadEmbeddedScript("chrome-api.js");
    }
    
    /// <summary>
    /// 从嵌入资源加载JavaScript脚本
    /// </summary>
    private void LoadEmbeddedScript(string scriptName)
    {
        try
        {
            string resourceName = $"{typeof(ChromeBridge).Namespace}.Resources.{scriptName}";
            Console.WriteLine($"[ChromeBridgeMVP] 加载脚本资源: {resourceName}");
            
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                string availableResources = string.Join(", ", resources);
                Console.WriteLine($"[ChromeBridgeMVP] 错误: 找不到资源 {resourceName}");
                Console.WriteLine($"[ChromeBridgeMVP] 可用资源: {availableResources}");
                return;
            }
            
            using StreamReader reader = new StreamReader(stream);
            string script = reader.ReadToEnd();
            
            _scriptEngine.Execute(script);
            Console.WriteLine($"[ChromeBridgeMVP] 成功加载脚本: {scriptName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ChromeBridgeMVP] 加载脚本 {scriptName} 时出错: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 执行测试脚本
    /// </summary>
    public string RunTest(string testName)
    {
        try
        {
            // 确保测试脚本已加载
            LoadEmbeddedScript("test-scripts.js");
            
            // 执行指定的测试函数
            var result = _scriptEngine.Execute($"{testName}()");
            Console.WriteLine($"[ChromeBridgeMVP] 执行测试 {testName} 返回: {result}");
            return result?.ToString() ?? "无返回值";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ChromeBridgeMVP] 执行测试 {testName} 时出错: {ex.Message}");
            return $"错误: {ex.Message}";
        }
    }
    
    /// <summary>
    /// 处理API调用并返回结果
    /// </summary>
    private JToken HandleApiCall(string apiName, object parameters)
    {
        // 显示详细参数信息
        string paramsStr = "null";
        if (parameters != null)
        {
            try { paramsStr = Newtonsoft.Json.JsonConvert.SerializeObject(parameters); }
            catch { paramsStr = parameters.ToString(); }
        }
        Console.WriteLine($"[ChromeBridgeMVP] 处理API: {apiName}, 参数: {paramsStr}");
        
        try
        {
            // 已实现的重点API方法
            switch (apiName)
            {
                case "tabs_query":
                    return TabsQuery(parameters);
                    
                case "tabs_get":
                    return TabsGet(parameters);
                    
                case "tabs_create":
                    return TabsCreate(parameters);
                
                // 简化实现API - 只记录调用，返回空对象
                default:
                    // 如果是api_开头的方法，返回模拟数据
                    if (apiName.StartsWith("api_") || apiName.Contains("_"))
                    {
                        Console.WriteLine($"[ChromeBridgeMVP] 调用API: {apiName} (简化实现)");
                        return RunBackgroundCommand(apiName, parameters);
                    }
                    
                    Console.WriteLine($"[ChromeBridgeMVP] 未实现的API: {apiName}");
                    return new JObject { ["error"] = $"未实现的API: {apiName}" };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ChromeBridgeMVP] 处理API时异常: {ex.Message}");
            return new JObject { ["error"] = ex.Message };
        }
    }

    public static JToken RunBackgroundCommand(string command, object parameters)
    {
        var respon = ChromeControl.RunBackgroundCommand("chrome", command, parameters, true, 500);
        if (respon.IsSuccess)
        {
            return respon.Data;
        }
        return new JObject();
    }

    /// <summary>
    /// 为各种API调用生成模拟结果
    /// </summary>
    private JToken GetMockResultForApi(string apiName, object parameters)
    {
        // 简单的结果模拟器
        if (apiName.Contains("_get") || apiName.Contains("_search") || apiName.Contains("_query"))
        {
            // 查询类API，返回类似数组的对象
            var result = new JObject();
            result["length"] = 2;
            result["0"] = new JObject { ["id"] = 1, ["title"] = "模拟结果1" };
            result["1"] = new JObject { ["id"] = 2, ["title"] = "模拟结果2" };
            return result;
        }
        else if (apiName.Contains("_create"))
        {
            // 创建类API，返回新创建的对象
            return new JObject { ["id"] = new Random().Next(100, 999), ["created"] = true };
        }
        else if (apiName.Contains("_update"))
        {
            // 更新类API，返回更新后的对象
            return new JObject { ["updated"] = true };
        }
        else if (apiName.Contains("_remove") || apiName.Contains("_delete"))
        {
            // 删除类API，返回成功状态
            return new JObject { ["success"] = true };
        }
        
        // 默认返回空对象
        return new JObject { ["success"] = true };
    }
    
    /// <summary>
    /// 查询标签页列表
    /// </summary>
    private JToken TabsQuery(object parameters)
    {
        // 为简化起见，忽略查询条件，返回所有标签页
        Console.WriteLine("[ChromeBridgeMVP] 执行tabs.query");
        
        // 创建返回结果 - 模拟JavaScript数组
        var result = new JObject();
        result["length"] = _tabs.Count;
        
        // 添加索引属性
        for (int i = 0; i < _tabs.Count; i++)
        {
            result[i.ToString()] = _tabs[i];
        }
        
        return result;
    }
    
    /// <summary>
    /// 获取特定标签页
    /// </summary>
    private JToken TabsGet(object parameters)
    {
        Console.WriteLine("[ChromeBridgeMVP] 执行tabs.get");
        
        try
        {
            // 解析标签页ID
            int tabId = -1;
            
            if (parameters is JObject jObj && jObj["tabId"] != null)
            {
                tabId = jObj["tabId"].Value<int>();
            }
            
            if (tabId <= 0)
            {
                return new JObject { ["error"] = "无效的标签页ID" };
            }
            
            // 查找标签页
            var tab = _tabs.FirstOrDefault(t => (int)t["id"] == tabId);
            if (tab == null)
            {
                return new JObject { ["error"] = $"找不到ID为 {tabId} 的标签页" };
            }
            
            return tab;
        }
        catch (Exception ex)
        {
            return new JObject { ["error"] = $"获取标签页异常: {ex.Message}" };
        }
    }
    
    /// <summary>
    /// 创建新标签页
    /// </summary>
    private JToken TabsCreate(object parameters)
    {
        Console.WriteLine("[ChromeBridgeMVP] 执行tabs.create");
        
        try
        {
            // 解析创建参数
            string url = "about:blank";
            string title = "New Tab";
            
            if (parameters is JObject jObj && jObj["url"] != null)
            {
                url = jObj["url"].ToString();
                title = $"Page at {url}";
            }
            
            // 创建新标签页
            int newId = _tabs.Count > 0 ? _tabs.Max(t => (int)t["id"]) + 1 : 1;
            
            var newTab = new JObject
            {
                ["id"] = newId,
                ["url"] = url,
                ["title"] = title
            };
            
            // 添加到标签页列表
            _tabs.Add(newTab);
            
            return newTab;
        }
        catch (Exception ex)
        {
            return new JObject { ["error"] = $"创建标签页异常: {ex.Message}" };
        }
    }
}
