using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotificationElementController : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Dismiss);
    }

    void OnDisable()
    {
        Destroy(this.gameObject);
    }

    public void Dismiss()
    {
        GetComponent<Animator>().SetTrigger("Dismissed");
    }
}
