using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameUIController : MonoBehaviour
{
    public void SetMark(int mark)
    {
        var endScreenMark = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        endScreenMark.text = mark.ToString();
    }
}
