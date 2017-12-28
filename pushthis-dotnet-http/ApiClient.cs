using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;

namespace Pushthis
{
    public class ApiClient : BaseClient
    {
        private string apiUrl = null;
        public Socket sock = null;
        private string socketName;

        /// <summary>
        /// Constructs a new ApiClient that can be used to handle authentication.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="authUrl">The serverUrl to use. Must end with /api!</param>
        public ApiClient(string key, string secret, string apiUrl, string socketName)
        {
            //if (!IsApiUrl(apiUrl))    // check if this is a valid auth url.
            //    throw new ArgumentException("The authUrl does not end with /auth and thus is invalid!");

            this.socketName = socketName;
            this.key = key;
            this.secret = secret;
            this.apiUrl = apiUrl;
            this.sock = IO.Socket(apiUrl, new IO.Options
            {
                //Query = new Dictionary<string, string> { { "key", this.key } },
                Transports = System.Collections.Immutable.ImmutableList.Create<string>("websocket"),
                Upgrade = false
                
            });   // TODO only if local
        }

        public void Connect(bool local = false)
        {
            this.sock.Connect();
            this.sock.On("info", (data) => Console.WriteLine($"[{socketName}] [INFO] " + JsonConvert.SerializeObject(data)));
        }

        public async Task SendRequestAsync(ApiRequest apiRequest)
        {           

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.apiUrl);
            req.ContentType = "application/json";
            req.Method = "POST";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                await sw.WriteAsync(apiRequest.Serialize());
            }
            var resp = await req.GetResponseAsync();
            using (var sr = new StreamReader(resp.GetResponseStream()))
            {
                var result = await sr.ReadToEndAsync();
            }

        }

        public void RegisterEvent(string eventName, Action<object> action)
        {
            this.sock.On(eventName, action);
        }


    }
}