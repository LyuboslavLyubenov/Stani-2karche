using UnityEngine.UI;
using System;

public class SecondsRemainingUIController : ExtendedMonoBehaviour
{
    public Text SecondsText;

    public int RemainingSecondsToAnswer
    {
        get;
        private set;
    }

    public bool Paused
    {
        get;
        set;
    }

    void Start()
    {
        Paused = false;

        CoroutineUtils.RepeatEverySeconds(1, () =>
            {
                if (RemainingSecondsToAnswer > 0 && !Paused)
                {
                    RemainingSecondsToAnswer--;
                }

                SecondsText.text = RemainingSecondsToAnswer.ToString();
            });
    }

    public void SetSeconds(int seconds)
    {
        if (seconds <= 0)
        {
            throw new ArgumentOutOfRangeException("seconds");
        }

        Paused = false;

        RemainingSecondsToAnswer = seconds;
        SecondsText.text = RemainingSecondsToAnswer.ToString();
    }
}
