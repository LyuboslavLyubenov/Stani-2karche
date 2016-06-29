using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class RequesterUtils
{
    public const string KinveyUrl = "http://baas.kinvey.com/";
    public const string AppKey = "kid_SJtjx5CS";
    public const string AppSecret = "82d0a91bc91c455e94415271a875d5ef";

    RequesterUtils()
    {
    }

    public static HttpWebRequest ConfigRequester(string url, string method, string data, bool useSession)
    {
        var request = WebRequest.Create(url) as HttpWebRequest;

        request.PreAuthenticate = true;
        request.Method = method;
        request.ReadWriteTimeout = 1000;

        if (useSession && PlayerPrefs.HasKey("SessionAuth"))
        {
            var tokenAuth = PlayerPrefs.GetString("SessionAuth");
            request.Headers.Set("Authorization", "Kinvey " + tokenAuth);
        }
        else
        {
            var credentials = Base64Encode(AppKey + ':' + AppSecret);
            request.Headers.Set("Authorization", "Basic " + credentials);    
        }

        request.Headers.Set("X-Kinvey-Api-Version", "3");

        if (!string.IsNullOrEmpty(data) && method != "GET")
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);

            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(dataBytes, 0, dataBytes.Length);
        }

        return request;
    }

    static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}
