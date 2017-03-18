namespace Controllers
{

    using UnityEngine.UI;

    using Utils.Unity;

    public class DialogController : ExtendedMonoBehaviour
    {
        private Text text;

        private bool initialized = false;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(1, this.Initialize);
        }

        private void Initialize()
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
