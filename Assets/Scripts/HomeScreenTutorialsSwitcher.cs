using System;
using System.IO;
using UnityEngine;

public class HomeScreenTutorialsSwitcher : DialogSwitcher
{
    const string ExplainGameTypesAlreadyPlayedKey = "TutorialShowed - HomeScreenTutorials - ExplainGameTypes";
    const string ExplainBasicExamModesAlreadyPlayedKey = "TutorialShowed - HomeScreenTutorials - ExplainBasicExamModes";

    public bool RepeatTutorials = false;

    protected override void Start()
    {
        string path = Directory.GetCurrentDirectory() + "\\LevelData\\homeScreenTutorials.txt";
        base.DialogFilePath = path;
        base.Start();
    }

    public void ExplainGameTypes()
    {
        if (PlayerPrefs.HasKey(ExplainGameTypesAlreadyPlayedKey) && !RepeatTutorials)
        {
            return;
        }

        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["HomeScreenGameTypes"];
                base.DisplayMessage(message, 2f);
                PlayerPrefs.SetInt(ExplainGameTypesAlreadyPlayedKey, 1);
            });
    }

    public void ExplainBasicExamModes()
    {
        if (PlayerPrefs.HasKey(ExplainBasicExamModesAlreadyPlayedKey) && !RepeatTutorials)
        {
            return;
        }

        CoroutineUtils.WaitForFrames(0, () =>
            {
                var message = base.TeacherDialogs["BasicExamModes"];
                base.DisplayMessage(message, 1.5f);
                PlayerPrefs.SetInt(ExplainBasicExamModesAlreadyPlayedKey, 1);
            });
    }
}
