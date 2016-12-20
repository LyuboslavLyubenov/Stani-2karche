using UnityEngine;
using UnityEngine.UI;

public class AddUICloack : MonoBehaviour
{
    Image cloackImage = null;

    public float Opacity = 0.5f;

    void Start()
    {
        var prefab = Resources.Load<Transform>("Prefabs/Cloack");
        var cloack = Instantiate(prefab);

        cloackImage = cloack.GetComponent<Image>();
        cloackImage.color = new Color(cloackImage.color.r, cloackImage.color.g, cloackImage.color.b, Opacity);

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
