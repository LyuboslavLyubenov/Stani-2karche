using UnityEngine.Networking;
using System;
using UnityEngine;

public class NetworkErrorUtils
{
    const string SuccessfullConnected = "Успешно свързан!";
    const string ConnectionProblem = "Проблем със свързването.";
    const string InvalidAddress = "Невалиден ип-адрес.";
    const string TooManyPlayers = "Домакинът няма свободни места.";
    const string AlreadyConnected = "Не може да се свържеш към повече от 1 домакин.";
    const string ConnectionBanned = "Домакинът е забранил да се свързваш към него.";
    const string CheckYourNetwork = "Моля проверете дали сте свързани към интернет и опитайте отново.";

    NetworkErrorUtils()
    {
    }

    public static string GetMessage(NetworkError error)
    {
        if (error == NetworkError.Ok)
        {
            return null;
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

        errorMessage += Environment.NewLine;
        errorMessage += Enum.GetName(typeof(NetworkError), error);

        return errorMessage;
    }

    public static string GetMessage(NetworkConnectionError error)
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

        errorMessage += Environment.NewLine;
        errorMessage += Enum.GetName(typeof(NetworkConnectionError), error);

        return errorMessage;
    }
}


