namespace Assets.Scripts.Controllers
{
    using UnityEngine;

    public class StartScreenController : MonoBehaviour
    {
        public GameObject CreateOrJoinUI;
        public EnterNameUIController EnterNameUIController;

        private void Start()
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

        private void OnUsernameSet(object sender, System.EventArgs args)
        {
            this.CreateOrJoinUI.SetActive(true);
        }
    }
}
