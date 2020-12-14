using Microsoft.AspNetCore.Http;
using AppServer.JSON_GS;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace AppServer
{
    public class AuthService
    {
        public static async Task<string> Auth(HttpContext context)
        {
            var msg = await JsonGsTools.GetMessage(context);
            if (msg == null) {context.Response.StatusCode = 404; return "";}
            context.Response.ContentType = "application/json";
            if (msg.Version != Message.VERSION) return Message.JsonGsErrorMessage(100);

            if (msg.Data.ContainsKey("Etoken") && msg.Data.ContainsKey("Dtoken"))
            {
                if (msg.Data["Etoken"] == "")
                {
                    using var connection = new SqlConnection(Startup.AppConfiguration.GetConnectionString("Auth"));
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand("sp_GetPlayerByDid", connection);
                    command.Parameters.AddWithValue("DID", "asdasdas");
                    command.CommandType = CommandType.StoredProcedure;
                    var reader = await command.ExecuteReaderAsync();
                    if (!reader.HasRows)
                        return (new Message(new Dictionary<string, string> 
                            { {"Result", "RegistrationRequired" }, {"Server", Startup.AppConfiguration["AppServers:Reg"]} }
                        )).ToJson();
                    else
                        // ToDo Генерация токена, сохранение в бд и отправка клиенту
                        return Message.JsonGsErrorMessage(777);
                }
                else
                {
                    // ToDo Обработка Etoken
                    return Message.JsonGsErrorMessage(777);
                }
            }
            else return Message.JsonGsErrorMessage(300);
        }
    }
}