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

    void OnLoggedIn(_UserReceivedData userData)
    {
        Debug.Log("Logged in successfuly ");
    }

    public void Login()
    {
        var username = UsernameInputField.text;
        var password = PasswordInputField.text;
        KinveyWrapper.Instance.LoginAsync(username, password, OnLoggedIn, Debug.LogException);
    }
}

