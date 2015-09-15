using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Security;

namespace Useful_Classes
{
    public static class WebConnection
    {
        public static string GetResponce(string url)
        {
            return GetResponse(new Uri(url), null, GetMethod.GET, null).ResponseData;
        }

        public static string GetResponce(string url, string username, string password)
        {
            return GetResponse(new Uri(url), new NetworkCredential(username, password), GetMethod.GET, null).ResponseData;
        }

        public static Response GetResponse(Uri url, NetworkCredential credential, GetMethod getMethod, WebHeaderCollection headers)
        {
            var data = new Response() { Success = false, ResponseData = "Unknown", HttpResponse = HttpStatusCode.NoContent, ResponseDescription = "Unknown" };
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Proxy = null;
                request.Method = getMethod.ToString();

                if (headers != null)
                    request.Headers = headers;

                if (credential != null)
                {
                    request.PreAuthenticate = true;
                    request.Credentials = credential;
                }
                else
                    request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                data.ResponseDescription = ((HttpWebResponse)response).StatusDescription;
                data.HttpResponse = ((HttpWebResponse)response).StatusCode;
                data.ResponseHeaders = ((HttpWebResponse)response).Headers;

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                data.ResponseData = reader.ReadToEnd();

                reader.Close();
                response.Close();

                data.Success = true;
            }
            catch (Exception ee)
            {
                data.Success = false;
                data.ResponseData = ee.ToString();
                data.ResponseDescription = ee.Message;
            }

            return data;
        }

        public enum GetMethod
        {
            GET,
            POST,
            PUT,
            DELETE,
            TRACE,
            CONNECT
        }
    }

    public class Response
    {
        public bool Success { get; set; }
        public HttpStatusCode HttpResponse { get; set; }
        public string ResponseDescription { get; set; }
        public string ResponseData { get; set; }
        public WebHeaderCollection ResponseHeaders { get; set; }

    }
}
