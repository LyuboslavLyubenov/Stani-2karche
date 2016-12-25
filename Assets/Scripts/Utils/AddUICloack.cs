using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{

    public class AddUICloack : MonoBehaviour
    {
        Image cloackImage = null;

        public float Opacity = 0.5f;

        void Start()
        {
            var prefab = Resources.Load<Transform>("Prefabs/Cloack");
            var cloack = Instantiate(prefab);

            this.cloackImage = cloack.GetComponent<Image>();
            this.cloackImage.color = new Color(this.cloackImage.color.r, this.cloackImage.color.g, this.cloackImage.color.b, this.Opacity);

            cloack.name = "Cloack " + this.transform.name;

            cloack.SetParent(this.transform.parent, false);
            this.transform.SetParent(cloack, false);
        }

        void OnDisable()
        {
            if (this.cloackImage != null)
            {
                this.cloackImage.enabled = false;
            }
        }

        void OnEnable()
        {
            if (this.cloackImage != null)
            {
                this.cloackImage.enabled = true;    
            }
        }
    }

}
