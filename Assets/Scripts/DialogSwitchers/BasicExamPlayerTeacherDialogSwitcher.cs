using UnityEngine;

namespace Assets.Scripts.DialogSwitchers
{

    public class BasicExamPlayerTeacherDialogSwitcher : DialogSwitcher
    {
        const string ExplainCategorySelectAlreadyPlayedKey = "TutorialShowed - BasicExamPlayerTeacher - ExplainCategorySelect";

        public bool RepeatTutorials = false;

        protected override void Initialize()
        {
            string teacherDialogsFilePath = "Tutorials Texts\\basicExamPlayerTeacherDialogs";
            base.DialogFilePath = teacherDialogsFilePath;
            base.Initialize();
        }

        public void ExplainCategorySelect()
        {
            if (PlayerPrefs.HasKey(ExplainCategorySelectAlreadyPlayedKey) && !this.RepeatTutorials)
            {
                return;
            }

            this.CoroutineUtils.WaitForFrames(0, () =>
                {
                    var message = base.TeacherDialogs["ExplainCategorySelect"];
                    base.DisplayMessage(message, 1.5f);
                    PlayerPrefs.SetInt(ExplainCategorySelectAlreadyPlayedKey, 1);
                });
        }
    }

}
