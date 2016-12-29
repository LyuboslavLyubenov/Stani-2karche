namespace Assets.Scripts.Utils.Unity.UI
{

    using Assets.Tests;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// Tooltip for ui elements. Wont work on 3d objects
    /// </summary>
    [RequireComponent(typeof(Renderer), typeof(RectTransform))]
    public class ActivateTooltip : ExtendedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const int Offset = 10;

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
        private bool showTooltip = false;

        private GameObject tooltip = null;

        private GameObject tooltipPrefab;

        private GameObject canvas;

        private RectTransform thisRectTransform;

        private void Start()
        {
            this.tooltipPrefab = Resources.Load<GameObject>("Prefabs\\Tooltip");
            this.canvas = GameObject.Find("Canvas");

            this.thisRectTransform = this.GetComponent<RectTransform>();

            if (this.InitializeOnStart)
            {
                this.Initialize();
            }
        }

        /// <summary>
        /// Creates tooltip window (with the given text.) 
        /// </summary>
        public void Initialize()
        {
            if (this.tooltip != null)
            {
                Destroy(this.tooltip);
                this.tooltip = null;
            }

            this.tooltip = Instantiate(this.tooltipPrefab);
            this.tooltip.transform.SetParent(this.canvas.transform, false);
            this.tooltip.name = "Tooltip " + this.transform.name;

            var tooltipRectTransform = this.tooltip.GetComponent<RectTransform>();

            var xOffset = 0;
            var yOffset = 0;

            var followObjComp = this.tooltip.GetComponent<FollowObj>();

            followObjComp.ObjToFollow = this.thisRectTransform;

            if (this.TooltipPosition == TooltipPosition.Left ||
                this.TooltipPosition == TooltipPosition.Right)
            {
                xOffset = (int)((this.thisRectTransform.rect.width / 2) + (this.SizeX / 2) + Offset);
                xOffset *= (this.TooltipPosition == TooltipPosition.Right) ? 1 : -1;

                followObjComp.FollowX = false;
                followObjComp.FollowY = true;
            }
            else
            {
                yOffset = (int)((this.thisRectTransform.rect.height / 2) + (this.SizeY / 2) + Offset);
                yOffset *= (this.TooltipPosition == TooltipPosition.Top) ? 1 : -1;

                followObjComp.FollowX = true;
                followObjComp.FollowY = false;
            }

            followObjComp.XOffset = xOffset;
            followObjComp.YOffset = yOffset;

            this.tooltip.transform.SetParent(this.thisRectTransform, false);

            tooltipRectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            tooltipRectTransform.sizeDelta = new Vector2(this.SizeX, this.SizeY);

            this.tooltip.GetComponentInChildren<Text>().text = this.Text;

            this.tooltip.transform.SetParent(this.canvas.transform, true);
        }

        //if mouse hover or finger is holding on current object
        //wait for DelayInSeconds to check if cursor is still on objects, if so activate tooltip
        public void OnPointerEnter(PointerEventData eventData)
        {
            //we should show tooltip
            this.showTooltip = true;
            this.CoroutineUtils.WaitForSeconds(this.DelayInSeconds, () => this.tooltip.SetActive(this.showTooltip));
        }

        //disable tooltip on mouse/finger exiting object
        public void OnPointerExit(PointerEventData eventData)
        {
            this.StopAllCoroutines();
            this.showTooltip = false;
            this.tooltip.SetActive(false);
        }

        private Rect GetPos()
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

}