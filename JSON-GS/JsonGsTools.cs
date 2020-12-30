using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AppServer.JSON_GS
{
    public class JsonGsTools
    {
        private static async Task<string> ReadContextBodyAsync(HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        private static string ReadContextBody(HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public static async Task<Message> GetMessageAsync(HttpContext context) => Message.FromJson(await ReadContextBodyAsync(context));

        public static Message GetMessage(HttpContext context) => Message.FromJson(ReadContextBody(context));

        public static string ObjectToJson(object obj) => JsonConvert.SerializeObject(obj);

        public static bool IsValidEmailAddress(string address) => address != null && new EmailAddressAttribute().IsValid(address);
    }
}
