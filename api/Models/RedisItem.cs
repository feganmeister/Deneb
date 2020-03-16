using Newtonsoft.Json.Linq;

namespace API.Models
{

    public class ResolveRedisType
        {
            public ResolveRedisType(string content)
            {
                dynamic json = JObject.Parse(content);
                RedisType = json["RedisType"];
            }
            public string RedisType { get; }
        }

    public class RedisHeader
    {
        public string RedisType { get; set; }
    }

    public class RedisItem1 : RedisHeader
    {
        public string Test1 { get; set; }
    }

    public class RedisItem2 : RedisHeader
    {
        public string Test2 { get; set; }
    }

}