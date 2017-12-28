using System;
using System.IO;

namespace Pushthis
{
    public class BaseClient
    {
        protected string pemPath = null;
        //private string channel = null;
        protected string key = null;
        protected string regionServerName = null;
        protected string secret = null;

        //public BaseClient(string key, string secret, string regionServerName)
        //{
        //    this.key = key;
        //    this.secret = secret;
        //    this.regionServerName = regionServerName;
        //}

        public void SetPem(string filePath)
        {
            if (File.Exists(filePath))
                this.pemPath = filePath;
            else
                throw new FileNotFoundException("The given pem file could not be found. Make sure the file exists.", filePath);
        }

        public bool IsAuthUrl(string url)
        {
            return url.EndsWith("/auth") || url.EndsWith("/auth/");
        }

        public bool IsApiUrl(string url)
        {
            return url.EndsWith("/api") || url.EndsWith("/api/");
        }
    }
}
