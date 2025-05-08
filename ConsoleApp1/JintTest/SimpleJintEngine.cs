using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using System;

namespace ConsoleApp1.JintTest
{
    /// <summary>
    /// 简单的Jint JavaScript引擎包装器
    /// </summary>
    public class SimpleJintEngine
    {
        private readonly Engine _engine;

        /// <summary>
        /// 初始化新的<see cref="SimpleJintEngine"/>实例
        /// </summary>
        public SimpleJintEngine()
        {
            _engine = new Engine(cfg => cfg.AllowClr()); // 允许.NET互操作
        }

        /// <summary>
        /// 将.NET对象注入JavaScript环境
        /// </summary>
        /// <param name="name">在JavaScript中暴露对象的名称</param>
        /// <param name="obj">.NET对象</param>
        public void InjectObject(string name, object obj)
        {
            _engine.SetValue(name, obj);
        }

        /// <summary>
        /// 将.NET静态类的方法和属性注入JavaScript环境
        /// </summary>
        /// <param name="name">在JavaScript中暴露静态类的名称</param>
        /// <param name="type">静态类的Type</param>
        public void InjectStaticClass(string name, Type type)
        {
            _engine.SetValue(name, TypeReference.CreateTypeReference(_engine, type));
        }

        /// <summary>
        /// 将.NET委托(方法)注入JavaScript环境
        /// </summary>
        /// <param name="name">在JavaScript中暴露函数的名称</param>
        /// <param name="func">.NET委托(例如Action, Func)注入</param>
        public void InjectFunction(string name, Delegate func)
        {
            _engine.SetValue(name, func);
        }

        /// <summary>
        /// 执行JavaScript代码
        /// </summary>
        /// <param name="script">JavaScript代码</param>
        /// <returns>执行结果</returns>
        public JsValue Execute(string script)
        {
            try
            {
                return _engine.Evaluate(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JavaScript执行错误: {ex.Message}");
                return JsValue.Undefined;
            }
        }

        /// <summary>
        /// 执行JavaScript代码并返回特定类型的结果
        /// </summary>
        /// <typeparam name="T">期望的返回类型</typeparam>
        /// <param name="script">JavaScript代码</param>
        /// <returns>转换为指定类型的执行结果</returns>
        public T? Execute<T>(string script)
        {
            try
            {
                JsValue result = Execute(script);
                
                if (result.IsUndefined() || result.IsNull())
                {
                    return default;
                }

                object? convertedResult = result.ToObject();

                if (convertedResult is T typedResult)
                {
                    return typedResult;
                }
                else
                {
                    try
                    {
                        return (T?)Convert.ChangeType(convertedResult, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine($"无法将结果转换为类型 {typeof(T)}");
                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行错误: {ex.Message}");
                return default;
            }
        }
    }
} 