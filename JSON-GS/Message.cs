using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppServer.JSON_GS
{
    public class Message : GsObject<Message>
    {
        public const string VERSION = "JSON-GS 1.0";
        public string Version;
        public string Header;       // -->"{JWT}"  <-- errorCode
        public Dictionary<string, string> Data;

        public Message() {}

        public Message(int errorCode)
        {
            Version = VERSION;
            switch (errorCode)
            {
                case 100: Header = "100 Unsupported protocol version."; break;
                case 200: Header = "200 Token timed out."; break;
                case 300: Header = "300 Wrong data."; break;
                case 500: Header = "500 Server error."; break;
                default:  Header = errorCode.ToString() + " Unknown error."; break;
            }
        }

        public Message(Dictionary<string, string> data)
        {
            Version = VERSION;
            Header = "000 OK";
            Data = data;
        }

        public static string JsonGsErrorMessage(int error) => (new Message(error)).ToJson();
    }

}
