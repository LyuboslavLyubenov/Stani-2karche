using UnityEngine.Networking;
using System;
using UnityEngine;
using System.Text;

public class NetworkErrorUtils
{
    const string SuccessfullConnected = "Успешно свързан!";
    const string SuccessfullySendOrReceiveMessage = "Успешно предадено/получено съобщение!";
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
            return SuccessfullySendOrReceiveMessage;
        }

        var errorMessage = new StringBuilder();
        var errorName = Enum.GetName(typeof(NetworkError), error);

        errorMessage.AppendLine(errorName);

        switch (error)
        {
            case NetworkError.DNSFailure:
                errorMessage.AppendLine(InvalidAddress);
                break;

            case NetworkError.Timeout:
                errorMessage.AppendLine(ConnectionProblem);
                break;
        }
         
        return errorMessage.ToString();
    }

    public static string GetMessage(NetworkConnectionError error)
    {
        var errorMessage = new StringBuilder();

        if (error == NetworkConnectionError.NoError)
        {
            errorMessage.AppendLine(SuccessfullConnected);
        }
        else
        {
            var errorName = Enum.GetName(typeof(NetworkConnectionError), error);

            errorMessage.AppendLine(errorName);

            switch (error)
            {
                case NetworkConnectionError.TooManyConnectedPlayers:
                    errorMessage.AppendLine(TooManyPlayers);
                    break;

                case NetworkConnectionError.AlreadyConnectedToServer:
                case NetworkConnectionError.AlreadyConnectedToAnotherServer:
                    errorMessage.AppendLine(AlreadyConnected);
                    break;

                case NetworkConnectionError.ConnectionBanned:
                    errorMessage.AppendLine(ConnectionBanned);
                    break;
            }
        }

        return errorMessage.ToString();
    }
}


