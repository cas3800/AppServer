using Microsoft.AspNetCore.Http;
using AppServer.JSON_GS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppServer
{
    public class InitService
    {
        public static async Task<string> Init(HttpContext context)
        {
            var msg = await JsonGsTools.GetMessageAsync(context);
            if (msg == null) { context.Response.StatusCode = 404; return ""; }
            context.Response.ContentType = "application/json";
            if (msg.Version != Message.VERSION) return Message.JsonGsErrorMessage(100);
            if (msg.Data.ContainsKey("Version"))
            {
                if (msg.Data["Version"] == Startup.AppConfiguration["AppVersion"])
                    return (new Message(new Dictionary<string, string>
                        { { "AuthServer", Startup.AppConfiguration["AppServers:Auth"] }, { "Result", "OK" } }
                    )).ToJson();
                else
                    return (new Message(new Dictionary<string, string>
                        { { "Result", "Unsupported version" } }
                    )).ToJson();
            }
            else return Message.JsonGsErrorMessage(300);
        }
    }
}