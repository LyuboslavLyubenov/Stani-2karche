using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class DialogController : ExtendedMonoBehaviour
    {
        Text text;

        bool initialized = false;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(1, this.Initialize);
        }

        void Initialize()
        {
            this.text = this.GetComponentInChildren<Text>();
            this.initialized = true;
        }

        public void SetMessage(string message)
        {
            this.CoroutineUtils.WaitUntil(() => this.initialized, () => this.text.text = message);
        }
    }

}
