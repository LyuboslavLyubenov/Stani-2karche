// ReSharper disable ArrangeTypeMemberModifiers
namespace Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{

    using UnityEngine;
    using UnityEngine.UI;

    public class MainPlayerUIController : MonoBehaviour
    {
        private const float OpacityLower = 0.25f;
        private const float OpacityHigh = 1f;

        private readonly Color OutlineColor = new Color(255, 200, 255, 0.5f);

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

        public bool RequestedGameStart
        {
            get
            {
                return this.requestedGameStart;
            }
            set
            {
                this.requestedGameStart = value;
                this.outline.enabled = value;
            }
        }

        private bool requestedGameStart;

        private Image image;
        private Text usernameText;
        private NicerOutline outline;

        void Start()
        {
            this.outline = this.gameObject.AddComponent<NicerOutline>();
            this.outline.effectColor = this.OutlineColor;
            this.outline.enabled = false;

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