using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FieldUIController : MonoBehaviour
{
    Text keyText;
    Text delimiterText;
    Text valueText;

    public string Key
    {
        get
        {
            return keyText.text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            transform.name = value;
            this.keyText.text = value;
        }
    }

    public string Delimiter
    {
        get
        {
            return delimiterText.text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            this.delimiterText.text = value;
        }
    }

    public string Value
    {
        get
        {
            return valueText.text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            this.valueText.text = value;
        }
    }

    void Start()
    {
        keyText = transform.Find("Key").GetComponent<Text>();
        delimiterText = transform.Find("Delimiter").GetComponent<Text>();
        valueText = transform.Find("Value").GetComponent<Text>();
    }
}
