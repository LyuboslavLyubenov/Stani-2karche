using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TestMyLuckUIController : ExtendedMonoBehaviour
{
    public EventHandler<JokerEventArgs> OnSelectedJoker = delegate
    {
    };

    const float DelayBetweenJokersRemovalInSeconds = 0.5f;
    const int XDistanceBetweenJokers = 20;
    const int YOffset = 20;

    Dictionary<IJoker, GameObject> jokersObjs = new Dictionary<IJoker, GameObject>();

    RectTransform upperContainer;
    RectTransform lowerContainer;

    GameObject jokerPrefab;

    bool initialized = false;

    void Start()
    {
        upperContainer = transform.Find("Upper").GetComponent<RectTransform>();
        lowerContainer = transform.Find("Lower").GetComponent<RectTransform>();
        jokerPrefab = Resources.Load<GameObject>("Prefabs/TestMyLuckUI/Joker");
    }

    IEnumerator InitializeCoroutine(IJoker[] jokers)
    {
        for (int i = 0; i < jokers.Length; i++)
        {
            var joker = jokers[i];
            var parent = (i % 2 == 0) ? upperContainer : lowerContainer;
            var obj = (GameObject)Instantiate(jokerPrefab, parent, false);
        
            obj.name = joker.GetType().Name;

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

        initialized = true;
    }

    IEnumerator SelectRandomJokerCoroutine()
    {
        yield return new WaitUntil(() => initialized);
        yield return new WaitForEndOfFrame();

        var jokers = jokersObjs.Keys.ToList();
        var selectedJoker = jokers.GetRandomElement();

        jokers.Remove(selectedJoker);
        jokers.Shuffle();

        for (int i = 0; i < jokers.Count; i++)
        {
            var joker = jokers[i];
            var obj = jokersObjs[joker];

            obj.GetComponent<Animator>().SetTrigger("disable");
            jokersObjs.Remove(joker);

            yield return new WaitForSeconds(DelayBetweenJokersRemovalInSeconds);
        }

        yield return new WaitForSeconds(1);

        StartCoroutine(ShowAsSelectedCoroutine(selectedJoker));
    }

    IEnumerator ShowAsSelectedCoroutine(IJoker joker)
    {
        var selectedJokerObj = jokersObjs[joker];
        selectedJokerObj.transform.SetParent(this.transform, true);

        yield return null;

        var rectTransform = selectedJokerObj.GetComponent<RectTransform>();
        var start = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0);
        var endX = this.GetComponent<RectTransform>().rect.width / 2;
        var end = new Vector3(endX, 0, 0);

        for (float i = 0f; i <= 1f; i += 0.075f)
        {
            var nextPosition = Vector3.Slerp(start, end, i);
            rectTransform.anchoredPosition = new Vector2(nextPosition.x, nextPosition.y);
            yield return null;
        }

        OnSelectedJoker(this, new JokerEventArgs(joker));
    }

    public void Initialize(IJoker[] jokers)
    {
        if (jokers == null || jokers.Length < 1)
        {
            throw new ArgumentException("jokers");
        }

        StartCoroutine(InitializeCoroutine(jokers));
    }

    public void SelectRandomJoker()
    {
        StartCoroutine(SelectRandomJokerCoroutine());
    }
}