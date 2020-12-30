using Microsoft.AspNetCore.Http;
using AppServer.JSON_GS;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using JWT;

namespace AppServer
{
    public class RegService
    {
        public static async Task<string> Reg(HttpContext context)
        {
            var msg = await JsonGsTools.GetMessageAsync(context);
            if (msg == null) { context.Response.StatusCode = 404; return ""; }
            context.Response.ContentType = "application/json";
            if (msg.Version != Message.VERSION) return Message.JsonGsErrorMessage(100);
            if (msg.Data.ContainsKey("Etoken") && msg.Data.ContainsKey("Dtoken")) return (msg.Data["Etoken"] == "") ? RegByDevice(msg, context) : RegByEmail(msg, context);
            if (msg.Data.ContainsKey("Etoken") && msg.Data.ContainsKey("Dtoken") && msg.Data.ContainsKey("NewEtoken")) return ChangePassword(msg, context);
            return Message.JsonGsErrorMessage(300);
        }

        private static string RegByDevice(Message msg, HttpContext context)
        {
            var DtokenPayload = JsonWebToken.DecodeToObject<Dictionary<string, string>>(msg.Data["Dtoken"], "", false);
            DtokenPayload.Add("IP", context.Connection.RemoteIpAddress.ToString());

            if (DtokenPayload["UI"].Length > 20)
            {
                using var connection = new MySqlConnection(Startup.AppConfiguration.GetConnectionString("Auth"));
                connection.Open();

                var command = new MySqlCommand("_NewPlayerByDevice", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("Device", DtokenPayload["UI"]);
                command.Parameters.AddWithValue("LogInfo", JsonGsTools.ObjectToJson(DtokenPayload));
                command.ExecuteNonQueryAsync();
            }
            return (new Message(new Dictionary<string, string>
                            { {"Result", "OK" } }
            )).ToJson();
        }

        private static string RegByEmail(Message msg, HttpContext context)
        {
            return "";
        }

        private static string ChangePassword(Message msg, HttpContext context)
        {
            return "";
        }
    }
}