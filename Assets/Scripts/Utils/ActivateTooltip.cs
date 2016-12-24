using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Tooltip for ui elements. Wont work on 3d objects
/// </summary>
[RequireComponent(typeof(Renderer), typeof(RectTransform))]
public class ActivateTooltip : ExtendedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    const int Offset = 10;

    /// <summary>
    /// Show tooltip if mouse/finger is still on this gameobject after this delay
    /// </summary>
    public int DelayInSeconds = 1;
    public int SizeX = 100;
    public int SizeY = 100;
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
    GameObject canvas;

    RectTransform thisRectTransform;

    void Start()
    {
        tooltipPrefab = Resources.Load<GameObject>("Prefabs\\Tooltip");
        canvas = GameObject.Find("Canvas");

        thisRectTransform = this.GetComponent<RectTransform>();

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
        tooltip.transform.SetParent(canvas.transform, false);
        tooltip.name = "Tooltip " + transform.name;

        var tooltipRectTransform = tooltip.GetComponent<RectTransform>();

        var xOffset = 0;
        var yOffset = 0;

        var followObjComp = tooltip.GetComponent<FollowObj>();

        followObjComp.ObjToFollow = thisRectTransform;

        if (TooltipPosition == TooltipPosition.Left ||
            TooltipPosition == TooltipPosition.Right)
        {
            xOffset = (int)((thisRectTransform.rect.width / 2) + (SizeX / 2) + Offset);
            xOffset *= (TooltipPosition == TooltipPosition.Right) ? 1 : -1;

            followObjComp.FollowX = false;
            followObjComp.FollowY = true;
        }
        else
        {
            yOffset = (int)((thisRectTransform.rect.height / 2) + (SizeY / 2) + Offset);
            yOffset *= (TooltipPosition == TooltipPosition.Top) ? 1 : -1;

            followObjComp.FollowX = true;
            followObjComp.FollowY = false;
        }

        followObjComp.XOffset = xOffset;
        followObjComp.YOffset = yOffset;

        tooltip.transform.SetParent(thisRectTransform, false);

        tooltipRectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
        tooltipRectTransform.sizeDelta = new Vector2(SizeX, SizeY);

        tooltip.GetComponentInChildren<Text>().text = Text;

        tooltip.transform.SetParent(canvas.transform, true);
    }

    //if mouse hover or finger is holding on current object
    //wait for DelayInSeconds to check if cursor is still on objects, if so activate tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        //we should show tooltip
        showTooltip = true;
        CoroutineUtils.WaitForSeconds(DelayInSeconds, () => tooltip.SetActive(showTooltip));
    }

    //disable tooltip on mouse/finger exiting object
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        showTooltip = false;
        tooltip.SetActive(false);
    }

    Rect GetPos()
    {
        return new Rect();
    }
}

public enum TooltipPosition
{
    Left,
    Right,
    Top,
    Bottom
}
