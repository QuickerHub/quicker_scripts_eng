using Newtonsoft.Json.Linq;
using Quicker.Utilities._3rd.Chrome;

namespace ChromeController;

public class Program
{
    public static JToken RunBackgroundCommand(string command, object parameters)
    {
        var respon = ChromeControl.RunBackgroundCommand("chrome", command, parameters, true, 500);
        if (respon.IsSuccess)
        {
            return respon.Data;
        }
        return new JObject();
    }

    public static void Exec(Quicker.Public.IStepContext context)
    {
        var data = RunBackgroundCommand("api_tabs_create", new { url = "https://baidu.com" });
        context.SetVarValue("result", data.ToString());
    }
}