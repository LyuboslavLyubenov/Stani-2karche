using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    public class ExitButtonController : MonoBehaviour
    {
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(this.LoadMainScreen);
        }

        void LoadMainScreen()
        {
            SceneManager.LoadScene("StartScreen");        
        }
    }

}
