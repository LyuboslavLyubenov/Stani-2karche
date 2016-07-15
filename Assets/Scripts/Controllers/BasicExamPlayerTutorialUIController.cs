using UnityEngine;
using UnityEngine.UI;

public class BasicExamPlayerTutorialUIController : MonoBehaviour
{
    public void Activate()
    {
        GetComponent<Image>().enabled = true;
        GetComponent<Animator>().enabled = true; 
    }
}
