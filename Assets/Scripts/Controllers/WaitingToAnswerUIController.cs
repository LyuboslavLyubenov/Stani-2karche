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
        disableAfterDelay = GetComponent<DisableAfterDelay>();
        remainingSecondsText = RemainingSecondsObject.GetComponent<Text>();
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
