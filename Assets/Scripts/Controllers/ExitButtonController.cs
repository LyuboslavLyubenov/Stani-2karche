namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Utils;

    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class ExitButtonController : MonoBehaviour
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(this.LoadMainScreen);
        }

        private void LoadMainScreen()
        {
            SceneManager.LoadScene("StartScreen");     
        }
    }
}
