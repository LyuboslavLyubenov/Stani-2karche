using System;
using UnityEngine;

public class BasicExamPlayerTeacherDialogSwitcher : DialogSwitcher
{
    const string ExplainCategorySelectAlreadyPlayedKey = "TutorialShowed - BasicExamPlayerTeacher - ExplainCategorySelect";

    public bool RepeatTutorials = false;

    protected override void Start()
    {
        string teacherDialogsFilePath = "Tutorials Texts\\basicExamPlayerTeacherDialogs";
        base.DialogFilePath = teacherDialogsFilePath;
        base.Start();
    }

    public void ExplainCategorySelect()
    {
        if (PlayerPrefs.HasKey(ExplainCategorySelectAlreadyPlayedKey) && !RepeatTutorials)
        {
            return;
        }

        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["ExplainCategorySelect"];
                base.DisplayMessage(message, 1.5f);
                PlayerPrefs.SetInt(ExplainCategorySelectAlreadyPlayedKey, 1);
            });
    }
}
