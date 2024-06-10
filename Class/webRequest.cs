using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace HeyaoChecker
{
    class webRequest
    {
        public static string get(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadString(url);
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string post(string url, string data)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    client.Encoding = Encoding.UTF8;
                    return client.UploadString(url, "POST", data);
                }
                catch
                {
                    return null;
                }
            }
        }



    }
}
