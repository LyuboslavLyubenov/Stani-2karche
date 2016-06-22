using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class DialogUIController : MonoBehaviour
{
    const string SuccessfullConnected = "Успешно свързан!";
    const string ConnectionProblem = "Проблем със свързването.";
    const string InvalidAddress = "Невалиден ип-адрес.";
    const string TooManyPlayers = "Домакинът няма свободни места.";
    const string AlreadyConnected = "Не може да се свържеш към повече от 1 домакин.";
    const string ConnectionBanned = "Домакинът е забранил да се свързваш към него.";
    const string CheckYourNetwork = "Моля проверете дали сте свързани към интернет и опитайте отново.";

    Text messageText = null;

    void Start()
    {
        messageText = gameObject.GetComponentInChildren<Text>();  
    }

    public void SetErrorMessage(string message)
    {
        StartCoroutine(SetMessageCoroutine(message));
    }

    public void SetErrorMessage(NetworkError error)
    {
        if (error == NetworkError.Ok)
        {
            return;
        }

        var errorMessage = ConnectionProblem;

        switch (error)
        {
            case NetworkError.DNSFailure:
                errorMessage += Environment.NewLine + InvalidAddress;
                break;

            case NetworkError.Timeout:
                errorMessage += Environment.NewLine + ConnectionProblem;
                break;
        }

        StartCoroutine(SetMessageCoroutine(errorMessage));
    }

    public void SetErrorMessage(NetworkConnectionError error)
    {
        var errorMessage = "";

        if (error == NetworkConnectionError.NoError)
        {
            errorMessage = SuccessfullConnected;
        }
        else
        {
            switch (error)
            {
                case NetworkConnectionError.TooManyConnectedPlayers:
                    errorMessage += Environment.NewLine + TooManyPlayers;
                    break;

                case NetworkConnectionError.AlreadyConnectedToServer:
                case NetworkConnectionError.AlreadyConnectedToAnotherServer:
                    errorMessage += Environment.NewLine + AlreadyConnected;
                    break;

                case NetworkConnectionError.ConnectionBanned:
                    errorMessage += Environment.NewLine + ConnectionBanned;
                    break;
            }
        }

        StartCoroutine(SetMessageCoroutine(errorMessage));
    }

    IEnumerator SetMessageCoroutine(string message)
    {
        yield return null;
        messageText.text = message;
    }
}
