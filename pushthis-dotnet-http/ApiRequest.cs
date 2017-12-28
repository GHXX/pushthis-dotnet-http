using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushthis
{
    public class ApiRequest
    {
        [JsonProperty]
        private string key = null;
        [JsonProperty]
        private string secret = null;
        [JsonProperty]
        private List<Dictionary<string, string>> payload = new List<Dictionary<string, string>>();

        public ApiRequest(string key, string secret)
        {
            this.key = key;
            this.secret = secret;
        }

        public ApiRequest AddPayload(string channel, string @event, object data)
        {
            payload.Add(
                new Dictionary<string, string>
                {
                    { "channel", channel },
                    { "event", @event },
                    { "data", JsonConvert.SerializeObject(data) }
                });

            return this;
        }

        public string Serialize()
        {
            if (payload.Count == 0)
                throw new InvalidOperationException("Cannot serialize APIRequest with zero payload objects.");

            return JsonConvert.SerializeObject(this);
            //return JsonConvert.SerializeObject(new Dictionary<string, string>
            //{
            //    {"key",this.key },
            //    {"secret",this.secret },
            //    {"payload",JsonConvert.SerializeObject(this.payload) }
            //});
        }
    }
}
