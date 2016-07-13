using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public abstract class DialogSwitcher : ExtendedMonoBehaviour
{
    public TeacherDialogController TeacherDialogController;
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

    protected virtual void Start()
    {
        if (TeacherDialogController == null)
        {
            throw new Exception("TeacherDialogController is null on " + this.gameObject.name);
        }    

        teacherDialogObj = TeacherDialogController.gameObject;

        var allTeacherDialogsFile = File.ReadAllLines(DialogFilePath);

        for (int i = 0; i < allTeacherDialogsFile.Length - 2; i += 3)
        {
            var tutorialName = allTeacherDialogsFile[i];
            var tutorialText = allTeacherDialogsFile[i + 1];

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
