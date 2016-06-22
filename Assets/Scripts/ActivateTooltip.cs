using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ActivateTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Tooltip;
    public int DelayInSeconds = 1;

    bool showTooltip = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        showTooltip = true;
        StartCoroutine(SetActiveTooltip());
    }

    IEnumerator SetActiveTooltip()
    {
        yield return new WaitForSeconds(DelayInSeconds);
        Tooltip.SetActive(showTooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        showTooltip = false;
        Tooltip.SetActive(false);
    }
        
}
