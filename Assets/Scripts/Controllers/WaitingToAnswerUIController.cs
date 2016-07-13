using UnityEngine;
using System;
using UnityEngine.UI;

public class WaitingToAnswerUIController : MonoBehaviour
{
    public GameObject RemainingSecondsObject;

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

        UpdateTimer();

        disableAfterDelay.OnTimePass += OnTimePass;
    }

    void OnDisable()
    {
        disableAfterDelay.OnTimePass -= OnTimePass;
    }

    void OnTimePass(object sender, EventArgs args)
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        remainingSecondsText.text = disableAfterDelay.DelayInSeconds - disableAfterDelay.PassedSeconds + " секунди";
    }
}
