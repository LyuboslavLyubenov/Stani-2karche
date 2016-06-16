using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DialogUIController : MonoBehaviour
{
    const string ConnetionProblem = "Проблем със свързването към сървъра.";
    const string InvalidAddress = "Невалиден ип-адрес.";

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
        var errorMessage = ConnetionProblem;
        
        switch (error)
        {
            case NetworkError.DNSFailure:
                errorMessage += "\n" + InvalidAddress;
                break;
        }

        StartCoroutine(SetMessageCoroutine(errorMessage));
    }

    IEnumerator SetMessageCoroutine(string message)
    {
        yield return null;
        messageText.text = message;
    }
}
