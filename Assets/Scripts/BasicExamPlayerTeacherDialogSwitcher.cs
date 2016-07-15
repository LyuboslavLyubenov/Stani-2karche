using System.IO;
using System;

public class BasicExamPlayerTeacherDialogSwitcher : DialogSwitcher
{
    protected override void Start()
    {
        string TeacherDialogsFilePath = Directory.GetCurrentDirectory() + "\\LevelData\\basicExamPlayerTeacherDialogs.txt";
        base.DialogFilePath = TeacherDialogsFilePath;
        base.Start();
    }

    public void ExplainThemeSelect()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["ExplainThemeSelect"];
                base.DisplayMessage(message, 1.5f);
            });
    }
}
