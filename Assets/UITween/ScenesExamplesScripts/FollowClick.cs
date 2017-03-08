using UnityEngine;

namespace Assets.UITween.ScenesExamplesScripts
{

    using Assets.UITween.Scripts;

    public class FollowClick : MonoBehaviour {

        public AnimationCurve LeftClick;
        public AnimationCurve RightClick;

        public EasyTween TweenToControl;
        public Transform RootCanvas;

        void Update ()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.MoveToMouseClick(this.LeftClick);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                this.MoveToMouseClick(this.RightClick);
            }
        }

        void MoveToMouseClick(AnimationCurve animationCurve)
        {
            Vector3 mouseAnchor = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            mouseAnchor = new Vector3(mouseAnchor.x * Screen.width / this.RootCanvas.localScale.x ,
                mouseAnchor.y * Screen.height / this.RootCanvas.localScale.y, 0f);

            if (!this.TweenToControl.IsObjectOpened())
            {
                this.TweenToControl.SetAnimationPosition(this.TweenToControl.rectTransform.anchoredPosition, (Vector2)mouseAnchor, animationCurve, animationCurve);
            }
            else
            {
                this.TweenToControl.SetAnimationPosition((Vector2)mouseAnchor, this.TweenToControl.rectTransform.anchoredPosition, animationCurve, animationCurve);
            }

            this.TweenToControl.OpenCloseObjectAnimation();
        }
    }

}
