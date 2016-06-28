using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class RequesterUtils
{
    const string KinveyUrl = "http://baas.kinvey.com/";
    const string AppKey = "kid_SJtjx5CS";
    const string AppSecret = "82d0a91bc91c455e94415271a875d5ef";

    RequesterUtils()
    {
    }

    public static HttpWebRequest ConfigRequester(string method, string data, bool useSession)
    {
        var endUrl = KinveyUrl + "user/" + AppKey + "/login";
        var request = WebRequest.Create(endUrl) as HttpWebRequest;

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

        if (!string.IsNullOrEmpty(data))
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(dataBytes, 0, dataBytes.Length);

            request.ContentType = "application/json";
            request.ContentLength = data.Length;
        }

        request.Method = method;
        request.Headers.Set("X-Kinvey-Api-Version", "3");

        return request;
    }

    static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}
