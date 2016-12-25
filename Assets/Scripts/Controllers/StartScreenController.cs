using UnityEngine;

namespace Assets.Scripts.Controllers
{

    public class StartScreenController : MonoBehaviour
    {
        public GameObject CreateOrJoinUI;
        public EnterNameUIController EnterNameUIController;

        void Start()
        {
            if (PlayerPrefs.HasKey("Username"))
            {
                this.EnterNameUIController.gameObject.SetActive(false);
                this.OnUsernameSet(null, System.EventArgs.Empty);
            }
            else
            {
                this.EnterNameUIController.gameObject.SetActive(true);
                this.EnterNameUIController.OnUsernameSet += this.OnUsernameSet;    
            }
        }

        void OnUsernameSet(object sender, System.EventArgs args)
        {
            this.CreateOrJoinUI.SetActive(true);
        }
    }

}
