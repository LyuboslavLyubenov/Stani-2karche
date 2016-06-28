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

    IEnumerator LoginAsync()
    {
        yield return null;

        //set user credentials
        var username = UsernameText.text;
        var password = PasswordText.text;
        var userCredentials = new _UserCredentials();

        userCredentials.username = username;
        userCredentials.password = password;

        var data = JsonUtility.ToJson(userCredentials);
        var requester = RequesterUtils.ConfigRequester("POST", data, false);

        //make login request to the server
        using (HttpWebResponse Response = requester.GetResponse() as HttpWebResponse)
        {
            if (Response.StatusCode != HttpStatusCode.OK)
            {
                //something went wrong
                throw new Exception("The request did not complete successfully and returned status code " + Response.StatusCode);   
            }                

            //login is successfull 
            using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
            {
                //parse received data
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

    public void Login()
    {
        this.StartCoroutineAsync(LoginAsync());
    }
}
