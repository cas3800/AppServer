using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace AppServer.JSON_GS
{
    public class JsonGsTools
    {
        private static async Task<string> ReadContextBody(HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        public static async Task<Message> GetMessage(HttpContext context) => Message.FromJson(await ReadContextBody(context));
    }
}
