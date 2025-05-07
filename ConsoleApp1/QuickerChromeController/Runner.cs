using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Quicker.Utilities._3rd.Chrome;

namespace QuickerChromeController
{
    public class Runner
    {

        public static BrowserRespMessage<JToken>? RunBackgroundScriptForMV3(string browser, string script,
                                                                           bool waitResponse, int timeoutMs,
                                                                           int mainProcId = 0,
                                                                           CancellationToken? cancellationToken = null)
        {
            BrowserRespMessage<JToken> runBackgroundCommandFunc(string commandName, object commandParam, bool waitResp, int timeoutMs)
            {
                return ChromeControl.RunBackgroundCommand(browser, commandName, commandParam, waitResp, timeoutMs, mainProcId,
                    null, cancellationToken);
            }

            //   // 因为需要多次调用，增加超时时间到原来的2倍吧
            //var executor = new ScriptExecutor(runBackgroundCommandFunc);
            //JToken result1 = executor.ExecuteScript(script, waitResponse, timeoutMs*2, cancellationToken);

            var engine = new ScriptEngineWrapper();

            // 创建简化版Chrome桥接
            var chrome = new ChromeBridge(engine, runBackgroundCommandFunc, timeoutMs, cancellationToken);


            ManualResetEvent? waitEvent = null;

            bool rtnIsSuccess = false;
            string rtnMessage = "";
            object? rtnData = null;


            if (waitResponse)
            {
                waitEvent = new ManualResetEvent(false);

                if (script.Contains("sendReplyToQuicker") == false)
                {
                    throw new Exception("代码中缺少sendReplyToQuicker调用");
                }
            }
            engine.InjectFunction("sendReplyToQuicker", (bool success, string message, object data, int msgSerial) =>
            {
                rtnIsSuccess = success;
                rtnMessage = message;
                rtnData = data;
                waitEvent?.Set();
            });



            engine.Execute(script.Replace("qk_msg_serial", "0"));

            //JsValue result = _jintEngine.Evaluate(script);
            //if (result == null || result.IsUndefined() || result.IsNull())
            //{
            //    return null;
            //}
            //// 如果结果是复杂对象，JToken.FromObject 能够很好地处理
            //return JToken.FromObject(result.ToObject());

            if (waitResponse)
            {
                try
                {
                    bool success = false;

                    if (cancellationToken.HasValue)
                    {


                        var index = WaitHandle.WaitAny(
                            [waitEvent, cancellationToken?.WaitHandle ?? CancellationToken.None.WaitHandle],
                            TimeSpan.FromMilliseconds(timeoutMs + 1));
                        success = index == 0;

                        if (!success)
                        {
                            if (index == WaitHandle.WaitTimeout)
                            {
                                throw new Exception($"操作超时未响应。");
                            }
                            else
                            {
                                throw new Exception($"用户取消。");

                            }

                        }
                    }
                    else
                    {
                        success = waitEvent!.WaitOne(timeoutMs + 1);
                    }

                    if (!success)
                    {
                        throw new Exception($"操作超时未响应。");
                    }

                    return new BrowserRespMessage<JToken>()
                    {
                        IsSuccess = rtnIsSuccess,
                        Message = rtnMessage,
                        Data = JToken.FromObject(rtnData!)
                    };
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException($"用户取消。");
                }
                finally
                {
                    // 删除event。
                    //_waitEvents.TryRemove(msg.Serial, out _);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
