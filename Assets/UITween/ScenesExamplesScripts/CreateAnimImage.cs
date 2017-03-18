namespace UITween.ScenesExamplesScripts
{

    using System.Collections.Generic;

    using UITween.Scripts;

    using UnityEngine;

    public class CreateAnimImage : MonoBehaviour {

        public CreateAnimImage[] createImageOtherReference;

        public GameObject CreateInstance;

        public int HowManyButtons;

        public Vector3 StartAnim;
        public Vector3 EndAnim;

        public float Offset;

        public AnimationCurve EnterAnim;
        public AnimationCurve ExitAnim;

        public RectTransform RootRect;
        public RectTransform RootCanvas;

        private List<EasyTween> Created = new List<EasyTween>();

        private Vector2 InitialCanvasScrollSize;
        private float totalWidth = 0f;

        void Start()
        {
            this.InitialCanvasScrollSize = new Vector2(this.RootRect.rect.height, this.RootRect.rect.width);
        }

        public void CallBack()
        {
            if (this.Created.Count == 0)
            {
                for (int i = 0; i < this.createImageOtherReference.Length; i++)
                {
                    this.createImageOtherReference[i].DestroyButtons();
                }

                this.CreateButtons();
            }
        }

        public void DestroyButtons()
        {
            for (int i = 0; i < this.Created.Count; i++)
            {
                this.Created[i].OpenCloseObjectAnimation();
            }

            this.Created.Clear();
        }

        public void CreateButtons()
        {
            this.CreatePanels();
            this.AdaptCanvas();
        }

        private void CreatePanels()
        {
            Vector3 InstancePosition = this.EndAnim;

            this.totalWidth = 0f;

            for (int i = 0; i < this.HowManyButtons; i++)
            {
                // Creates Instance
                GameObject createInstance = Instantiate(this.CreateInstance) as GameObject;
                // Changes the Parent, Assing to scroll List
                createInstance.transform.SetParent(this.RootRect, false);
                EasyTween easy = createInstance.GetComponent<EasyTween>();
                // Add Tween To List
                this.Created.Add(easy);
                // Final Position
                this.StartAnim.y = InstancePosition.y;
                // Pass the positions to the Tween system
                easy.SetAnimationPosition(this.StartAnim, InstancePosition , this.EnterAnim, this.ExitAnim);
                // Intro fade
                easy.SetFade();
                // Execute Animation
                easy.OpenCloseObjectAnimation();
                // Increases the Y offset
                InstancePosition.y += this.Offset;

                this.totalWidth += this.Offset;
            }
        }

        private void AdaptCanvas()
        {
            // Vertical Dynamic Adapter
            if (this.InitialCanvasScrollSize.x < Mathf.Abs(this.totalWidth) )
            {
                this.RootRect.offsetMin = new Vector2(this.RootRect.offsetMin.x, this.totalWidth + this.InitialCanvasScrollSize.x + this.RootRect.offsetMax.y);
            }
        }
    }

}