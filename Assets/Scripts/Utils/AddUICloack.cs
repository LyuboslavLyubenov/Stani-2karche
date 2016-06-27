using UnityEngine;
using UnityEngine.UI;

public class AddUICloack : MonoBehaviour
{
    Image cloackImage = null;

    void Start()
    {
        var canvas = GameObject.Find("Canvas");
        var prefab = Resources.Load<Transform>("Prefabs/Cloack");
        var cloack = Instantiate(prefab);

        cloackImage = cloack.GetComponent<Image>();

        cloack.name = "Cloack " + transform.name;
        cloack.SetParent(transform.parent, false);
        transform.SetParent(cloack, false);
    }

    void OnDisable()
    {
        if (cloackImage != null)
        {
            cloackImage.enabled = false;
        }
    }

    void OnEnable()
    {
        if (cloackImage != null)
        {
            cloackImage.enabled = true;    
        }
    }
}
