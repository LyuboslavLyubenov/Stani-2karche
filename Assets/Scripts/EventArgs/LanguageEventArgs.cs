using System;
using System.IO;
using System.Xml;
using UnityEngine;

public class LanguageEventArgs : EventArgs
{
    public string Language
    {
        get;
        private set;
    }

    public LanguageEventArgs(string language)
    {
        if (string.IsNullOrEmpty(language))
        {
            throw new ArgumentNullException(language);
        }
        
        this.Language = language;
    }
}
