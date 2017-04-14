namespace Reporter
{

    using UnityEngine;

    public class ReporterGUI : MonoBehaviour
    {
        private Reporter reporter;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Awake()
        {
            this.reporter = this.gameObject.GetComponent<Reporter>();
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnGUI()
        {
            this.reporter.OnGUIDraw();
        }
    }

}
