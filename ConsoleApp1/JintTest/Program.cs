using ConsoleApp1.JintTest;
using System;

namespace ConsoleApp1.JintTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Jint ES6类支持测试程序\n");
            
            // 根据命令行参数或者直接调用运行测试
            string testOption = args.Length > 0 ? args[0].ToLower() : "class";
            
            switch(testOption)
            {
                case "class":
                    JintClassTest.RunTest();
                    break;
                    
                case "simple":
                    SimpleEngineTest();
                    break;
                    
                default:
                    Console.WriteLine("未知的测试选项。可用选项: class, simple");
                    break;
            }
        }
        
        static void SimpleEngineTest()
        {
            Console.WriteLine("测试SimpleJintEngine\n");
            
            var engine = new SimpleJintEngine();
            
            // 执行一些简单的JavaScript代码
            var result1 = engine.Execute<int>("2 + 3");
            Console.WriteLine($"2 + 3 = {result1}");
            
            var result2 = engine.Execute<string>("'Hello, ' + 'World!'");
            Console.WriteLine($"字符串拼接: {result2}");
            
            // 测试ES6特性
            try
            {
                var classResult = engine.Execute<string>(@"
                    class Greeter {
                        constructor(name) {
                            this.name = name;
                        }
                        
                        greet() {
                            return `Hello, ${this.name}!`;
                        }
                    }
                    
                    //const g = 
new Greeter('Jint').greet();
                    //g.greet();
                ");
                
                Console.WriteLine($"ES6类测试结果: {classResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ES6类测试失败: {ex.Message}");
            }
        }
    }
}
