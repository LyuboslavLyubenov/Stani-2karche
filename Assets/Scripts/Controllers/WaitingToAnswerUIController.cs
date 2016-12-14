using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class WaitingToAnswerUIController : MonoBehaviour
{
    public GameObject RemainingSecondsObject;
    public ClientNetworkManager NetworkManager;

    Text remainingSecondsText;
    DisableAfterDelay disableAfterDelay = null;

    void OnEnable()
    {
        if (RemainingSecondsObject == null)
        {
            throw new NullReferenceException("RemainingSecondsObjects is null on WaitingToAnswerUIController obj");
        }

        disableAfterDelay = GetComponent<DisableAfterDelay>();

        if (disableAfterDelay == null)
        {
            throw new Exception("WaitingToAnswerUIController obj must have DisableAfterDelay component");
        }

        remainingSecondsText = RemainingSecondsObject.GetComponent<Text>();

        if (remainingSecondsText == null)
        {
            throw new Exception("RemainingSecondsObject obj is null or doesnt have Text component");
        }

        UpdateTimer(disableAfterDelay.DelayInSeconds);

        disableAfterDelay.OnTimePass += OnTimePass;
    }

    void OnDisable()
    {
        disableAfterDelay.OnTimePass -= OnTimePass;
    }

    void UpdateTimer(int remainingSeconds)
    {
        remainingSecondsText.text = remainingSeconds + " секунди";
    }

    void OnTimePass(object sender, TimeInSecondsEventArgs args)
    {
        UpdateTimer(args.Seconds);
    }
}
