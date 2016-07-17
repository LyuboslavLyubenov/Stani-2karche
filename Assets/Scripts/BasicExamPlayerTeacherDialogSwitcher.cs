using System;
using UnityEngine;

public class BasicExamPlayerTeacherDialogSwitcher : DialogSwitcher
{
    const string ExplainThemeSelectAlreadyPlayedKey = "TutorialShowed - BasicExamPlayerTeacher - ExplainThemeSelect";

    public bool RepeatTutorials = false;

    protected override void Start()
    {
        string TeacherDialogsFilePath = Application.dataPath + "\\Tutorials Texts\\basicExamPlayerTeacherDialogs.txt";
        base.DialogFilePath = TeacherDialogsFilePath;
        base.Start();
    }

    public void ExplainThemeSelect()
    {
        if (PlayerPrefs.HasKey(ExplainThemeSelectAlreadyPlayedKey) && !RepeatTutorials)
        {
            return;
        }

        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["ExplainThemeSelect"];
                base.DisplayMessage(message, 1.5f);
                PlayerPrefs.SetInt(ExplainThemeSelectAlreadyPlayedKey, 1);
            });
    }
}
