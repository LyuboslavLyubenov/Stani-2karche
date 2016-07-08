using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Used to activate tooltip on start menu
/// </summary>
public class ActivateTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    const int Offset = 10;

    /// <summary>
    /// Show tooltip if mouse/finger is still on this gameobject after this delay
    /// </summary>
    public int DelayInSeconds = 1;
    public int SizeX = 300;
    public int SizeY = 300;
    public string Text;
    public TooltipPosition TooltipPosition = TooltipPosition.Top;

    //should show tooltip
    bool showTooltip = false;
    GameObject Tooltip = null;

    void Start()
    {
        GameObject tooltipPrefab = Resources.Load<GameObject>("Prefabs\\Tooltip");

        Tooltip = Instantiate(tooltipPrefab);
        Tooltip.transform.SetParent(this.transform, false);
        Tooltip.name = "Tooltip " + transform.name;


        var rectTransform = Tooltip.GetComponent<RectTransform>();
        var anchorsMin = new Vector2();
        var anchorsMax = new Vector2();
        var size = new Vector2();
        var position = new Vector2();

        switch (TooltipPosition)
        {
            case TooltipPosition.Left:

                anchorsMax = new Vector2(0, 1);
                size = new Vector2(SizeX, rectTransform.sizeDelta.y);
                position = new Vector2(-((size.x / 2) + Offset), 0);

                break;

            case TooltipPosition.Right:

                anchorsMin = new Vector2(1, 0);
                anchorsMax = new Vector2(1, 1);
                size = new Vector2(SizeX, rectTransform.sizeDelta.y);
                position = new Vector2((size.x / 2) + Offset, 0);

                break;

            case TooltipPosition.Top:
                
                anchorsMin = new Vector2(0, 1);
                anchorsMax = new Vector2(1, 1);
                size = new Vector2(rectTransform.sizeDelta.x, SizeY);
                position = new Vector2(0, (size.y / 2) + Offset);

                break;

            case TooltipPosition.Bottom:

                anchorsMax = new Vector2(1, 0);
                size = new Vector2(rectTransform.sizeDelta.x, SizeY);
                position = new Vector2(0, -((size.y / 2) + Offset));

                break;

        }

        rectTransform.anchorMin = anchorsMin;
        rectTransform.anchorMax = anchorsMax;
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        Tooltip.GetComponentInChildren<Text>().text = Text;
    }

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
        StopAllCoroutines();
        showTooltip = false;
        Tooltip.SetActive(false);
    }
        
}

public enum TooltipPosition
{
    Left,
    Right,
    Top,
    Bottom
}
