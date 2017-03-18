namespace Controllers
{

    using System;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Utils.Unity;

    public class GameTypesUIController : MonoBehaviour
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            //TODO: Remove me
            this.LoadNormalGame();
        }

        public void LoadNormalGame()
        {
            PlayerPrefsEncryptionUtils.SetString("MainPlayerHost", "true");
            PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", "127.0.0.1");
            PlayerPrefsEncryptionUtils.SetString("ServerExternalIP", "127.0.0.1");

            SceneManager.LoadScene("BasicExamMainPlayer", LoadSceneMode.Single);    
        }

        public void LoadAudienceRevenge()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void LoadFastestWins()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void QuizDuel()
        {
            //TODO
            throw new NotImplementedException();
        }
    }

}
