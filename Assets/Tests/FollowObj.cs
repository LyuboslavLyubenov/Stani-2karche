using UnityEngine;
using System.Collections;

public class FollowObj : MonoBehaviour
{
    public RectTransform ObjToFollow;

    public int XOffset = 0;
    public int YOffset = 0;

    public bool FollowX = true;
    public bool FollowY = true;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        var newX = rectTransform.position.x;
        var newY = rectTransform.position.y;

        if (FollowX)
        {
            newX = ObjToFollow.position.x + XOffset;
        }

        if (FollowY)
        {
            newY = ObjToFollow.position.y + YOffset;
        }

        var newPos = new Vector2(newX, newY);
        rectTransform.position = newPos;
    }
}