using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ChromeController;

public static class Runner
{
    // 静态成员存储脚本引擎和Chrome桥接
    private static readonly ScriptEngineWrapper _engine;
    private static readonly ChromeBridge _chrome;

    // 静态构造函数，初始化脚本引擎和Chrome桥接
    static Runner()
    {
        // 创建脚本引擎
        _engine = new ScriptEngineWrapper();

        // 创建简化版Chrome桥接
        _chrome = new ChromeBridge(_engine);

        // 注入日志函数
        _engine.InjectFunction("log", new Action<string>(message =>
        {
            Console.WriteLine($"[JS日志] {message}");
        }));
    }

    public static JToken RunBackgrounScripts(string scripts)
    {
        try
        {
            // 执行脚本
            var result = _engine.Execute(scripts);
            
            // 将结果转换为JToken
            if (result == null)
            {
                return JValue.CreateNull();
            }
            
            // 通过JSON序列化和反序列化进行转换
            string json = JsonConvert.SerializeObject(result);
            return JToken.Parse(json);
        }
        catch (Exception ex)
        {
            // 记录异常并返回错误信息
            Console.WriteLine($"执行脚本时出现异常: {ex.Message}");
            
            // 创建包含错误信息的JObject
            var errorResult = new JObject
            {
                ["success"] = false,
                ["error"] = ex.Message,
                ["stackTrace"] = ex.StackTrace
            };
            
            return errorResult;
        }
    }
}