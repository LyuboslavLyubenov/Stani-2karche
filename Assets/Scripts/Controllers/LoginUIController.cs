using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System;
using CielaSpike;
using UnityEngine.UI;

public class LoginUIController : MonoBehaviour
{
    public Text UsernameText;
    public Text PasswordText;

    void Start()
    {
        this.StartCoroutineAsync(LoginAsync());
    }

    IEnumerator LoginAsync()
    {
        yield return null;

        var username = UsernameText.text;
        var password = PasswordText.text;
        var userCredentials = new _UserCredentials();

        userCredentials.username = username;
        userCredentials.password = password;

        var data = JsonUtility.ToJson(userCredentials);
        var requester = RequesterUtils.ConfigRequester("POST", data, false);

        using (HttpWebResponse Response = requester.GetResponse() as HttpWebResponse)
        {
            if (Response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("The request did not complete successfully and returned status code " + Response.StatusCode);   
            }                

            using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
            {
                string returnDataJSON = Reader.ReadToEnd();

                yield return Ninja.JumpToUnity;

                var retrieveData = JsonUtility.FromJson<_UserRetrievedData>(returnDataJSON);
                OnLoggedIn(retrieveData);

                yield return Ninja.JumpBack;
            }
        }

        yield return null;
    }

    void OnLoggedIn(_UserRetrievedData userData)
    {
        
    }
}
