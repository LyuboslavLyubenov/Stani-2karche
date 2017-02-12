// ReSharper disable ArrangeTypeMemberModifiers
namespace Assets.Scripts.Controllers.PlayersConnecting
{
    using UnityEngine;
    using UnityEngine.UI;

    public class MainPlayerUIController : MonoBehaviour
    {
        private const float OpacityLower = 0.25f;
        private const float OpacityHigh = 1f;

        public string Username
        {
            get
            {
                return this.usernameText.text;
            }
            set
            {
                this.SetOpacityToMaximum();
                this.usernameText.text = value;
            }
        }

        private Image image;
        private Text usernameText;
        
        void Start()
        {
            this.image = this.GetComponent<Image>();
            this.usernameText = this.GetComponentInChildren<Text>();
            this.SetOpaticyToMinimum();
        }

        private void SetOpaticyToMinimum()
        {
            var imageColor = this.image.color;
            this.image.color = new Color(imageColor.r, imageColor.g, imageColor.b, OpacityLower);
        }

        private void SetOpacityToMaximum()
        {
            var imageColor = this.image.color;
            this.image.color = new Color(imageColor.r, imageColor.g, imageColor.b, OpacityHigh);
        }
        
        public void ClearUsername()
        {
            this.SetOpaticyToMinimum();
            this.usernameText.text = string.Empty;
        }
    }
}