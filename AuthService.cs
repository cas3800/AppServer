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
    public class AuthService
    {
        public static async Task<string> Auth(HttpContext context)
        {
            var msg = await JsonGsTools.GetMessageAsync(context);
            if (msg == null) { context.Response.StatusCode = 404; return ""; }
            context.Response.ContentType = "application/json";
            if (msg.Version != Message.VERSION) return Message.JsonGsErrorMessage(100);
            if (!msg.Data.ContainsKey("Etoken") || !msg.Data.ContainsKey("Dtoken")) return Message.JsonGsErrorMessage(300);

            var DtokenPayload = JsonWebToken.DecodeToObject<Dictionary<string, string>>(msg.Data["Dtoken"], "", false);
            DtokenPayload.Add("IP", context.Connection.RemoteIpAddress.ToString());

            using var connection = new MySqlConnection(Startup.AppConfiguration.GetConnectionString("Auth"));
            connection.Open();

            int PlayerId = 0;
            if (msg.Data["Etoken"] != "")
            {
                MySqlCommand command = new MySqlCommand("_GetPlayerByEtoken", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("Etoken", msg.Data["Etoken"]);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    PlayerId = reader.GetInt32(0);
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    command = new MySqlCommand("_InsertEtokenFlood", connection) { CommandType = CommandType.StoredProcedure };
                    command.Parameters.AddWithValue("Etoken", msg.Data["Etoken"]);
                    command.Parameters.AddWithValue("Dtoken", msg.Data["Dtoken"]);
                    command.Parameters.AddWithValue("LogInfo", JsonGsTools.ObjectToJson(DtokenPayload));
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                    command.ExecuteNonQueryAsync();
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                    return (new Message(new Dictionary<string, string>
                            { {"Result", "EtokenError" } }
                    )).ToJson();
                }
            }

            if (PlayerId == 0)
            {
                MySqlCommand command = new MySqlCommand("_GetPlayerByDevice", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("Device", DtokenPayload["UI"]);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    PlayerId = reader.GetInt32(0);
                }
                reader.Close();
            }

            if (PlayerId > 0)
            {
                var Token = JsonWebToken.Encode(new Dictionary<string, string> { { "ID", PlayerId.ToString() } }, "", JwtHashAlgorithm.GS);
                MySqlCommand command = new MySqlCommand("_SetToken", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("PlayerId", PlayerId);
                command.Parameters.AddWithValue("Token", Token);
                command.Parameters.AddWithValue("LogInfo", JsonGsTools.ObjectToJson(DtokenPayload));
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                command.ExecuteNonQueryAsync();
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                return (new Message(new Dictionary<string, string>
                            { {"Result", "OK" }, {"Token", Token } }
                )).ToJson();
            }
            else
                return (new Message(new Dictionary<string, string>
                            { {"Result", "RegistrationRequired" }, {"Server", Startup.AppConfiguration["AppServers:Reg"]} }
                )).ToJson();
        }
    }
}