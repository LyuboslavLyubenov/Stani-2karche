using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class SelectRandomJokerUIController : ExtendedMonoBehaviour
{
    public EventHandler<JokerTypeEventArgs> OnFinishedAnimation = delegate
    {
    };

    const float DelayBetweenJokersRemovalInSeconds = 0.9f;
    const float DelayShowSelectedJoker = 3f;
    const int XDistanceBetweenJokers = 20;
    const int YOffset = 20;

    Dictionary<Type, GameObject> jokersObjs = new Dictionary<Type, GameObject>();

    RectTransform upperContainer;
    RectTransform lowerContainer;

    RectTransform thisRectTransform;

    GameObject jokerPrefab;

    void Start()
    {
        upperContainer = transform.Find("Upper").GetComponent<RectTransform>();
        lowerContainer = transform.Find("Lower").GetComponent<RectTransform>();
        thisRectTransform = transform.GetComponent<RectTransform>();
        jokerPrefab = Resources.Load<GameObject>("Prefabs/SelectRandomJoker/Joker");
    }

    IEnumerator CreateJokerObjs(Type[] jokers)
    {
        yield return null;

        jokersObjs.Clear();
            
        for (int i = 0; i < jokers.Length; i++)
        {
            var joker = jokers[i];
            var parent = (i % 2 == 0) ? upperContainer : lowerContainer;
            var obj = (GameObject)Instantiate(jokerPrefab, parent, false);
            var jokerName = joker.Name.Replace("Joker", "");

            obj.name = jokerName;

            var jokerImagePath = JokerConstants.JokerImagesPath + jokerName;
            var jokerImage = Resources.Load<Sprite>(jokerImagePath);
            var imgObj = obj.transform.GetChild(0);

            imgObj.GetComponent<Image>().sprite = jokerImage;

            var rectTransform = obj.GetComponent<RectTransform>();
            var parentJokersCount = (int)Math.Ceiling(jokers.Length / 2f);
            var xSize = ((parent.rect.width - (parentJokersCount * XDistanceBetweenJokers)) / parentJokersCount);
            var ySize = parent.rect.height - YOffset;
            var size = new Vector2(xSize, ySize);

            rectTransform.sizeDelta = size;

            var xPos = (xSize / 2f) + (XDistanceBetweenJokers / 2f) + (xSize + XDistanceBetweenJokers) * (parent.childCount - 1);
            var pos = new Vector2(xPos, 0);

            rectTransform.anchoredPosition = pos;

            jokersObjs.Add(joker, obj);

            yield return null;
        }
    }

    IEnumerator DeleteNotSelectedJokers(Type[] notSelectedJokers)
    {
        var jokersToDelete = notSelectedJokers.ToArray();
        jokersToDelete.Shuffle();

        for (int i = 0; i < jokersToDelete.Length; i++)
        {
            yield return new WaitForEndOfFrame();

            var joker = jokersToDelete[i];
            var obj = jokersObjs[joker];

            obj.GetComponent<Animator>().SetTrigger("destroy");
            jokersObjs.Remove(joker);

            yield return new WaitForSeconds(DelayBetweenJokersRemovalInSeconds);
        }
    }

    IEnumerator PlaySelectRandomJokerCoroutine(Type[] jokers, int selectedJokerIndex)
    {
        yield return StartCoroutine(CreateJokerObjs(jokers));
        yield return new WaitForEndOfFrame();

        var selectedJoker = jokers[selectedJokerIndex];
        var jokersToDelete = jokers.Where(j => j != selectedJoker).ToArray();
         
        yield return StartCoroutine(DeleteNotSelectedJokers(jokersToDelete));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(ShowAsSelectedCoroutine(selectedJoker));
    }

    IEnumerator ShowAsSelectedCoroutine(Type joker)
    {
        var selectedJokerObj = jokersObjs[joker];
        selectedJokerObj.transform.SetParent(this.transform, true);

        yield return null;

        var rectTransform = selectedJokerObj.GetComponent<RectTransform>();
        var start = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0);
        var endX = thisRectTransform.rect.width / 2;
        var end = new Vector3(endX, 0, 0);

        for (float i = 0f; i <= 1f; i += 0.075f)
        {
            var nextPosition = Vector3.Slerp(start, end, i);
            rectTransform.anchoredPosition = new Vector2(nextPosition.x, nextPosition.y);
            yield return null;
        }

        yield return new WaitForSeconds(DelayShowSelectedJoker);

        Destroy(selectedJokerObj);

        yield return new WaitForSeconds(1f);

        OnFinishedAnimation(this, new JokerTypeEventArgs(joker));

        this.gameObject.SetActive(false);
    }

    public void PlaySelectRandomJokerAnimation(Type[] jokers, int selectedJokerIndex)
    {
        if (jokers == null ||
            jokers.Length < 1)
        {
            throw new ArgumentException("jokers");
        }

        if (selectedJokerIndex < 0 || selectedJokerIndex >= jokers.Length)
        {
            throw new ArgumentOutOfRangeException("selectedJokerIndex");
        }

        StartCoroutine(PlaySelectRandomJokerCoroutine(jokers, selectedJokerIndex));
    }
}