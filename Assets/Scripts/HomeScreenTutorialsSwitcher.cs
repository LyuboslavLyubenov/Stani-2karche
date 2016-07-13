using System;
using System.IO;

public class HomeScreenTutorialsSwitcher : DialogSwitcher
{
    protected override void Start()
    {
        string path = Directory.GetCurrentDirectory() + "\\LevelData\\homeScreenTutorials.txt";
        base.DialogFilePath = path;
        base.Start();
    }

    public void ExplainGameTypes()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["HomeScreenGameTypes"];
                base.DisplayMessage(message, 3);
            });
    }

    public void ExplainBasicExamModes()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["BasicExamModes"];
                base.DisplayMessage(message, 2);
            });
    }
}
