using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotificationElementController : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Dismiss);
        StartCoroutine(DismissAfterDelay());
    }

    IEnumerator DismissAfterDelay()
    {
        yield return new WaitForSeconds(5);
        Dismiss();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public void Dismiss()
    {
        GetComponent<Animator>().SetTrigger("Dismissed");
    }
}
