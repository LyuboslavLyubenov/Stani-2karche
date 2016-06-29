using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System;
using CielaSpike;
using UnityEngine.UI;

public class LoginUIController : MonoBehaviour
{
    public InputField UsernameInputField;
    public InputField PasswordInputField;

    IEnumerator LoginAsync(string username, string password)
    {
        //set user credentials
        var userCredentials = new _UserCredentials();

        userCredentials.username = username;
        userCredentials.password = password;

        var data = JsonUtility.ToJson(userCredentials);
        var url = RequesterUtils.KinveyUrl + "user/" + RequesterUtils.AppKey + "/login";
        var requester = RequesterUtils.ConfigRequester(url, "POST", data, false);

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
    }

    void OnLoggedIn(_UserRetrievedData userData)
    {
        Debug.Log("Logged in successfuly ");
    }

    public void Login()
    {
        //this.StartCoroutineAsync(LoginAsync());
    }
}
