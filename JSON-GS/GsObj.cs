using Newtonsoft.Json;

namespace AppServer.JSON_GS
{
    public class GsObj<T>
    {
        public string ToJson() => JsonConvert.SerializeObject(this);

        public static T FromJson(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}
