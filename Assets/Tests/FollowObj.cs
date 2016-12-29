using UnityEngine;

namespace Assets.Tests
{

    public class FollowObj : MonoBehaviour
    {
        public RectTransform ObjToFollow;

        public int XOffset = 0;
        public int YOffset = 0;

        public bool FollowX = true;
        public bool FollowY = true;

        private RectTransform rectTransform;

        private void Start()
        {
            this.rectTransform = this.gameObject.GetComponent<RectTransform>();
        }

        private void Update()
        {
            var newX = this.rectTransform.position.x;
            var newY = this.rectTransform.position.y;

            if (this.FollowX)
            {
                newX = this.ObjToFollow.position.x + this.XOffset;
            }

            if (this.FollowY)
            {
                newY = this.ObjToFollow.position.y + this.YOffset;
            }

            var newPos = new Vector2(newX, newY);
            this.rectTransform.position = newPos;
        }
    }

}