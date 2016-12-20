using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Used to activate tooltip on start menu
/// </summary>
public class ActivateTooltip : ExtendedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    /// <summary>
    /// Calls initialize method on start.
    /// </summary>
    public bool InitializeOnStart = true;

    //should show tooltip
    bool showTooltip = false;
    GameObject tooltip = null;

    GameObject tooltipPrefab;

    void Start()
    {
        tooltipPrefab = Resources.Load<GameObject>("Prefabs\\Tooltip");

        if (InitializeOnStart)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Creates tooltip window (with the given text.) 
    /// </summary>
    public void Initialize()
    {
        if (tooltip != null)
        {
            Destroy(tooltip);
            tooltip = null;
        }

        tooltip = Instantiate(tooltipPrefab);
        tooltip.transform.SetParent(this.transform, false);
        tooltip.name = "Tooltip " + transform.name;

        var rectTransform = tooltip.GetComponent<RectTransform>();
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

        tooltip.GetComponentInChildren<Text>().text = Text;
    }

    //if mouse hover or finger is holding on current object
    //wait for DelayInSeconds to check if cursor is still on objects, if so activate tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        //we should show tooltip
        showTooltip = true;
        CoroutineUtils.WaitForSeconds(1f, () => tooltip.SetActive(showTooltip));
    }

    //disable tooltip on mouse/finger exiting object
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        showTooltip = false;
        tooltip.SetActive(false);
    }
        
}

public enum TooltipPosition
{
    Left,
    Right,
    Top,
    Bottom
}
