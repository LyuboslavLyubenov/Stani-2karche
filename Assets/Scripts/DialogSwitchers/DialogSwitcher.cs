namespace Assets.Scripts.DialogSwitchers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using UnityEngine;

    using Controllers;
    using Utils.Unity;

    public abstract class DialogSwitcher : ExtendedMonoBehaviour
    {
        public DialogController TeacherDialogController;

        /// <summary>
        /// Path to dialogs file starting from Resources folder
        /// </summary>
        public string DialogFilePath;

        private GameObject teacherDialogObj;

        private Dictionary<string, string> teacherDialogs = new Dictionary<string, string>();

        private bool initialized = false;

        public bool Initialized
        {
            get
            {
                return this.initialized;
            }
        }

        protected Dictionary<string,string> TeacherDialogs
        {
            get
            {
                if (!this.initialized)
                {
                    return null;
                }

                return this.teacherDialogs;
            }
        }

        protected virtual void Initialize()
        {
            this.teacherDialogObj = this.TeacherDialogController.gameObject;

            string[] dialogFileLines;

#if UNITY_ANDROID
        var file = Resources.Load<TextAsset>(DialogFilePath).text;
        dialogFileLines = file.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        #else
            dialogFileLines = File.ReadAllLines(this.DialogFilePath);
#endif
            
            for (int i = 0; i < dialogFileLines.Length - 2; i += 3)
            {
                var tutorialName = dialogFileLines[i];
                var tutorialText = dialogFileLines[i + 1];

                this.teacherDialogs.Add(tutorialName, tutorialText);
            }

            this.initialized = true;
        }

        protected void DisplayMessage(string message, float delayInSeconds)
        {
            this.StartCoroutine(this.DisplayMessageCoroutine(message, delayInSeconds));
        }

        private IEnumerator DisplayMessageCoroutine(string message, float delayInSeconds)
        {
            yield return new WaitUntil(() => this.initialized);
            yield return new WaitForSeconds(delayInSeconds);

            this.teacherDialogObj.SetActive(true);
            this.TeacherDialogController.SetMessage(message);
        }
    }

}
