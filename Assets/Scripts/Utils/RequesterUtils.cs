namespace Assets.Scripts.Utils
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class RequesterUtils
    {
        public const string KinveyUrl = "https://baas.kinvey.com/";
        public const string AppKey = "kid_B1p9ERZMl";
        public const string AppSecret = "4d8b75c1cd254ce293409aaa43c25981";

        public static string SessionAuth
        {
            get;
            set;
        }

        private RequesterUtils()
        {
        }

        ~RequesterUtils()
        {
            RemoveSessionAuth();
        }

        public static void RemoveSessionAuth()
        {
            SessionAuth = null;
        }

        public static HttpWebRequest ConfigRequester(string url, string method, string data, bool useSession)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

            var request = WebRequest.Create(url) as HttpWebRequest;

            request.PreAuthenticate = true;
            request.Method = method;
            request.ReadWriteTimeout = 2000;

            if (useSession)
            {
                if (string.IsNullOrEmpty(SessionAuth))
                {
                    throw new InvalidOperationException("Doest have session auth. Not logged in");
                }

                request.Headers.Set("Authorization", "Kinvey " + SessionAuth);
            }
            else
            {
                var credentials = Base64Encode(AppKey + ':' + AppSecret);
                request.Headers.Set("Authorization", "Basic " + credentials);    
            }

            request.Headers.Set("X-Kinvey-Api-Version", "3");

            if (!string.IsNullOrEmpty(data) && method != "GET")
            {
                request.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);    
                }
            }

            return request;
        }

        /// <summary>
        /// Usage ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        /// Use it if you want to establish secure connection to server (https)
        /// </summary>
        private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}