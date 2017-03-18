namespace Controllers
{

    using System;

    using EventArgs;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class EnterNameUIController : ExtendedMonoBehaviour
    {
        public GameObject UsernameTextField;
        public EventHandler<UserNameEventArgs> OnUsernameSet = delegate
            {
            };

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            if (this.UsernameTextField == null)
            {
                throw new NullReferenceException("UsernameTextField is null on EnterNameUIController obj");
            }

            this.CoroutineUtils.WaitForFrames(0, this.Initialize);
        }

        private void Initialize()
        {
            if (PlayerPrefsEncryptionUtils.HasKey("Username"))
            {
                var username = PlayerPrefsEncryptionUtils.GetString("Username");
                this.OnUsernameSet(this, new UserNameEventArgs(username));
                this.gameObject.SetActive(false);
            }
        }

        private void Deactivate()
        {
            this.GetComponent<Animator>().SetTrigger("disable");
        }

        public void SaveUsername()
        {
            var usernameText = this.UsernameTextField.GetComponent<Text>();
            this.SaveUsername(usernameText.text);
        }

        public void SaveUsername(string username)
        {
            PlayerPrefsEncryptionUtils.SetString("Username", username);
            this.Deactivate();
            this.OnUsernameSet(this, new UserNameEventArgs(username));        
        }
    }

}