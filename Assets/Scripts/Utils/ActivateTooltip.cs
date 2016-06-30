using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Used to activate tooltip on start menu
/// </summary>
public class ActivateTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Tooltip panel
    /// </summary>
    public GameObject Tooltip;

    /// <summary>
    /// Show tooltip if mouse/finger is still on this gameobject
    /// </summary>
    public int DelayInSeconds = 1;

    //should show tooltip
    bool showTooltip = false;

    //if mouse hover or finger is holding on current object
    public void OnPointerEnter(PointerEventData eventData)
    {
        //we should show tooltip
        showTooltip = true;
        StartCoroutine(SetActiveTooltip());
    }

    //wait for DelayInSeconds to check if cursor is still on objects, if so activate tooltip
    IEnumerator SetActiveTooltip()
    {
        yield return new WaitForSeconds(DelayInSeconds);
        Tooltip.SetActive(showTooltip);
    }

    //disable tooltip on mouse/finger exiting object
    public void OnPointerExit(PointerEventData eventData)
    {
        showTooltip = false;
        Tooltip.SetActive(false);
    }
        
}
