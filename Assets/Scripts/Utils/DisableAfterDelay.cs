using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class DisableAfterDelay : MonoBehaviour
{
    public int DelayInSeconds;
    public int PassedSeconds;
    public bool DisableAfterClick = true;
    public bool UseAnimator = false;

    public EventHandler OnTimePass = delegate
    {
        
    };

    public event EventHandler OnTimeEnd
    {
        add
        {
            if (onTimeEnd == null || !onTimeEnd.GetInvocationList().Contains(value))
            {
                onTimeEnd += value;
            }
        }
        remove
        {
            onTimeEnd -= value;
        }
    }

    EventHandler onTimeEnd = delegate
    {
    };

    void OnEnable()
    {
        StartCoroutine(DisableWithDelay());
    }

    void OnDisable()
    {
        PassedSeconds = 0;
        onTimeEnd(this, EventArgs.Empty);
    }

    void FixedUpdate()
    {
        if (DisableAfterClick && Input.GetMouseButton(0))
        {
            StopCoroutine(DisableWithDelay());
            Disable();
        }
    }

    void Disable()
    {
        if (UseAnimator)
        {
            GetComponent<Animator>().SetTrigger("disable");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator DisableWithDelay()
    {
        while (PassedSeconds <= DelayInSeconds)
        {
            yield return new WaitForSeconds(1f);
            PassedSeconds++;
            OnTimePass(this, EventArgs.Empty);
        }

        Disable();
    }
}
