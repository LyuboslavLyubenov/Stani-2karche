namespace Utils.Unity.UI
{

    using UnityEngine;
    using UnityEngine.UI;

    public class AddUICloack : MonoBehaviour
    {
        private Image cloackImage = null;

        public float Opacity = 0.5f;

        private void Start()
        {
            var prefab = Resources.Load<Transform>("Prefabs/Cloack");
            var cloack = Instantiate(prefab);

            this.cloackImage = cloack.GetComponent<Image>();
            this.cloackImage.sprite = null;
            this.cloackImage.color = new Color(this.cloackImage.color.r, this.cloackImage.color.g, this.cloackImage.color.b, this.Opacity);

            cloack.name = "Cloack " + this.transform.name;

            cloack.SetParent(this.transform.parent, false);
            this.transform.SetParent(cloack, false);
        }

        private void OnDisable()
        {
            if (this.cloackImage != null)
            {
                this.cloackImage.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (this.cloackImage != null)
            {
                this.cloackImage.enabled = true;    
            }
        }
    }

}
