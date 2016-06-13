using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogUIController : MonoBehaviour
{
    Text messageText = null;

    void Start()
    {
        messageText = gameObject.GetComponentInChildren<Text>();  
    }

    public void SetMessage(string message)
    {
        StartCoroutine(SetMessageCoroutine(message));
    }

    IEnumerator SetMessageCoroutine(string message)
    {
        yield return null;
        messageText.text = message;
    }
}
