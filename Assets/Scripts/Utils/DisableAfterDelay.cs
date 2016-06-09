using UnityEngine;
using System.Collections;
using System;

public class DisableAfterDelay : MonoBehaviour
{
    public int DelayInSeconds;
    public int PassedSeconds;
    public bool DisableAfterClick = true;

    public EventHandler OnTimePass = delegate
    {
        
    };
    public EventHandler OnTimeEnd = delegate
    {
        
    };

    void OnEnable()
    {
        StartCoroutine(DisableWithDelay());
    }

    void OnDisable()
    {
        PassedSeconds = 0;
        OnTimeEnd(this, EventArgs.Empty);
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            StopCoroutine(DisableWithDelay());
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

        gameObject.SetActive(false);
    }
}
