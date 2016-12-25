using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Notifications
{

    public class NotificationElementController : MonoBehaviour, IPointerUpHandler
    {
        public int WaitBeforeDisableSeconds = 5;

        void Start()
        {
            this.StartCoroutine(this.DismissAfterDelay());
        }

        IEnumerator DismissAfterDelay()
        {
            yield return new WaitForSeconds(this.WaitBeforeDisableSeconds);
            this.Dismiss();
        }

        void OnDisable()
        {
            this.StopAllCoroutines();
            Destroy(this.gameObject);
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
