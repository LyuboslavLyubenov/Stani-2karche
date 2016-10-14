using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.Events;

public class ChoosedCategoryEventArgs : EventArgs
{
    public string Name
    {
        get;
        private set;
    }

    public string Path
    {
        get;
        private set;
    }

    public ChoosedCategoryEventArgs(string name, string path)
        : base()
    {
        if (name == null)
        {
            throw new ArgumentNullException("name");
        }
            
        if (path == null)
        {
            throw new ArgumentNullException("path");
        }

        this.Name = name;
        this.Path = path;
    }
}