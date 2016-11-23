using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;

public static class HttpWebRequesterExtensions
{
    public static string GetRequestResult(this HttpWebRequest requester)
    {
        using (HttpWebResponse response = requester.GetResponse() as HttpWebResponse)
        {
            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.Created &&
                response.StatusCode != HttpStatusCode.NoContent &&
                response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception(response.StatusCode + " " + response.StatusDescription);  
            }   

            using (StreamReader Reader = new StreamReader(response.GetResponseStream()))
            {
                string dataJSON = Reader.ReadToEnd();
                return dataJSON;
            }
        }
    }
}
