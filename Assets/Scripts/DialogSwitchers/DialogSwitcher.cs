using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public abstract class DialogSwitcher : ExtendedMonoBehaviour
{
    public DialogController TeacherDialogController;

    /// <summary>
    /// Path to dialogs file starting from Resources folder
    /// </summary>
    public string DialogFilePath;

    GameObject teacherDialogObj;

    Dictionary<string, string> teacherDialogs = new Dictionary<string, string>();
    bool initialized = false;

    public bool Initialized
    {
        get
        {
            return initialized;
        }
    }

    protected Dictionary<string,string> TeacherDialogs
    {
        get
        {
            if (!initialized)
            {
                return null;
            }

            return teacherDialogs;
        }
    }

    protected virtual void Initialize()
    {
        teacherDialogObj = TeacherDialogController.gameObject;

        string[] dialogFileLines;

        #if UNITY_ANDROID
        var file = Resources.Load<TextAsset>(DialogFilePath).text;
        dialogFileLines = file.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        #else
        dialogFileLines = File.ReadAllLines(DialogFilePath);
        #endif
            
        for (int i = 0; i < dialogFileLines.Length - 2; i += 3)
        {
            var tutorialName = dialogFileLines[i];
            var tutorialText = dialogFileLines[i + 1];

            teacherDialogs.Add(tutorialName, tutorialText);
        }

        initialized = true;
    }

    protected void DisplayMessage(string message, float delayInSeconds)
    {
        StartCoroutine(DisplayMessageCoroutine(message, delayInSeconds));
    }

    IEnumerator DisplayMessageCoroutine(string message, float delayInSeconds)
    {
        yield return new WaitUntil(() => initialized);
        yield return new WaitForSeconds(delayInSeconds);

        teacherDialogObj.SetActive(true);
        TeacherDialogController.SetMessage(message);
    }
}
