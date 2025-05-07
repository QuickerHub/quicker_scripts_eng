using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using System.Diagnostics;

namespace QuickerChromeController
{
    /// <summary>
    /// Wraps the Jint JavaScript engine.
    /// </summary>
    public class ScriptEngineWrapper
    {
        private readonly Engine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptEngineWrapper"/> class.
        /// </summary>
        public ScriptEngineWrapper()
        {
            _engine = new Engine(cfg => cfg.AllowClr()); // Allow CLR interop
        }

        /// <summary>
        /// Injects a .NET object into the JavaScript environment.
        /// </summary>
        /// <param name="name">The name to expose the object under in JavaScript.</param>
        /// <param name="obj">The .NET object to inject.</param>
        public void InjectObject(string name, object obj)
        {
            _engine.SetValue(name, obj);
        }

        /// <summary>
        /// Injects a .NET static class's methods and properties into the JavaScript environment.
        /// </summary>
        /// <param name="name">The name to expose the static class under in JavaScript.</param>
        /// <param name="type">The Type of the static class.</param>
        public void InjectStaticClass(string name, Type type)
        {
            if (!type.IsAbstract || !type.IsSealed) // Heuristic check for static class
            {
                // Jint typically needs an instance, but we can expose static members via TypeReference
                // However, directly using Type might be complex for JS.
                // A common pattern is to create a wrapper or expose static methods individually.
                // For simplicity, let's expose the Type object itself.
                // Users can then access static members like: StaticClassName.StaticMethod()
                // Note: This requires AllowClr() which we have enabled.
                _engine.SetValue(name, TypeReference.CreateTypeReference(_engine, type));

                // Alternative: Inject specific static methods as functions if needed
                // foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                // {
                //     if (!method.IsSpecialName) // Avoid property getters/setters etc.
                //     {
                //         _engine.SetValue($"{name}_{method.Name}", Delegate.CreateDelegate(typeof(Action), method)); // Adjust delegate type based on method signature
                //     }
                // }
            }
            else
            {
                Console.WriteLine($"Warning: Type '{type.Name}' is not a static class. Injecting type reference.");
                _engine.SetValue(name, TypeReference.CreateTypeReference(_engine, type));
            }
        }


        /// <summary>
        /// Injects a .NET delegate (method) into the JavaScript environment.
        /// </summary>
        /// <param name="name">The name to expose the function under in JavaScript.</param>
        /// <param name="func">The .NET delegate (e.g., Action, Func) to inject.</param>
        public void InjectFunction(string name, Delegate func)
        {
            _engine.SetValue(name, func);
        }

        /// <summary>
        /// Executes a JavaScript code string.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <returns>The result of the script execution.</returns>
        public JsValue Execute(string script)
        {
            try
            {
                return _engine.Evaluate(script);
            }
            catch (Exception ex)
            {
                throw;
                Debug.WriteLine($"JavaScript Error: {ex.Message}");
                // Optionally rethrow or return an error indicator
                return JsValue.Undefined;
            }
        }

        /// <summary>
        /// Executes a JavaScript code string and returns the result as a specific type.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <returns>The result of the script execution, converted to type T.</returns>
        public T? Execute<T>(string script)
        {
            try
            {
                JsValue result = _engine.Evaluate(script);
                // Attempt to convert the JsValue result to the desired .NET type
                // Jint provides mechanisms for conversion, including ToObject()
                // or specific methods like AsString(), AsNumber(), etc.
                // Need to handle potential conversion errors.
                if (result.IsUndefined() || result.IsNull())
                {
                    return default; // Return default value for the type T if result is null/undefined
                }

                // Use Jint's built-in conversion capabilities
                object? convertedResult = result.ToObject();

                if (convertedResult is T typedResult)
                {
                    return typedResult;
                }
                else
                {
                    // Attempt explicit conversion if direct casting fails
                    try
                    {
                        return (T?)Convert.ChangeType(convertedResult, typeof(T));
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine($"JavaScript Error: Could not convert result to type {typeof(T)}. Details: {ex.Message}");
                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JavaScript Error: {ex.Message}");
                return default;
            }
        }
    }
}
