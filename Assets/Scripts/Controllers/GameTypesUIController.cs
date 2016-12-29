namespace Assets.Scripts.Controllers
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
            this.transform.localScale = new Vector3(1, 0, 1);
        }

        public void LoadNormalGame()
        {
            PlayerPrefsEncryptionUtils.SetString("MainPlayerHost", "true");
            PlayerPrefsEncryptionUtils.SetString("ServerIP", "127.0.0.1");

            PlayerPrefsEncryptionUtils.SetString("ServerMaxPlayers", "30");

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
