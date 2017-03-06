namespace Assets.Scripts.Notifications
{
    using System.Collections;

    using Assets.Scripts.Interfaces.Notifications;

    using UnityEngine;
    using UnityEngine.EventSystems;

    public class NotificationElementController : MonoBehaviour, INotificationElementController, IPointerUpHandler
    {
        public int WaitBeforeDisableSeconds = 5;
        
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.StartCoroutine(this.DismissAfterDelay());
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnDisable()
        {
            this.StopAllCoroutines();
            Destroy(this.gameObject);
        }

        private IEnumerator DismissAfterDelay()
        {
            yield return new WaitForSeconds(this.WaitBeforeDisableSeconds);
            this.Dismiss();
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            this.Dismiss();
        }

        public void Dismiss()
        {
            this.GetComponent<Animator>().SetTrigger("Dismissed");
        }
    }
}
