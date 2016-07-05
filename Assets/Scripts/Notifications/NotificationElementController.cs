using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotificationElementController : MonoBehaviour, IPointerUpHandler
{
    public int WaitBeforeDisableSeconds = 5;

    void Start()
    {
        StartCoroutine(DismissAfterDelay());
    }

    IEnumerator DismissAfterDelay()
    {
        yield return new WaitForSeconds(WaitBeforeDisableSeconds);
        Dismiss();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Dismiss();
    }

    public void Dismiss()
    {
        GetComponent<Animator>().SetTrigger("Dismissed");
    }
}
