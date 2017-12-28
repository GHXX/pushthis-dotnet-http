using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pushthis
{
    public class AuthClient : BaseClient
    {
        private string authUrl = null;

        /// <summary>
        /// Constructs a new AuthClient that can be used to handle authentication.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="authUrl">The serverUrl to use. Must end with /auth!</param>
        public AuthClient(string key, string secret, string authUrl)
        {
            if (!IsAuthUrl(authUrl))    // check if this is a valid auth url.
                throw new ArgumentException("The authUrl does not end with /auth and thus is invalid!");

            this.key = key;
            this.secret = secret;
            this.authUrl = authUrl;
        }

        public async Task Authorize(string channelName, string socketName, bool allow)
        {
            // prepare content
            string payload = JsonConvert.SerializeObject(
                new Dictionary<string, string>
                {
                    { "channel", channelName },
                    { "authorized", allow.ToString() },
                    { "socket_id", socketName }
                });

            string data = JsonConvert.SerializeObject(
                new Dictionary<string, string>
            {
                { "key", this.key },
                { "secret", this.secret },
                { "payload", payload }
            });

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.authUrl);
            req.ContentType = "application/json";
            req.Method = "POST";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                await sw.WriteAsync(data);
            }
            var resp = await req.GetResponseAsync();
            using (var sr = new StreamReader(resp.GetResponseStream()))
            {
                var result = await sr.ReadToEndAsync();
            }
        }
    }
}
