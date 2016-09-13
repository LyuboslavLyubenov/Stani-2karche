using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class AvailableJokersUIController : MonoBehaviour
{
    const int SpawnOffset = 10;

    public Transform Container;

    RectTransform containerRectTransform;

    List<IJoker> availableJokers = new List<IJoker>();
    Transform dummyJokerButtonPrefab;
    Vector2 jokerStartPosition;
    Vector2 jokerButtonSize;

    float distanceBetweenJokers = 0;

    void Start()
    {
        containerRectTransform = Container.GetComponent<RectTransform>();
        dummyJokerButtonPrefab = Resources.Load<Transform>("Prefabs/DummyJokerButton");

        var rectTransform = dummyJokerButtonPrefab.GetComponent<RectTransform>();
        jokerButtonSize = rectTransform.sizeDelta;
        jokerStartPosition = rectTransform.anchoredPosition;
        distanceBetweenJokers = jokerStartPosition.y - jokerButtonSize.y;
    }

    public void AddJoker(IJoker joker)
    {
        var jokerButtonObj = Instantiate(dummyJokerButtonPrefab);

        jokerButtonObj.SetParent(Container, false);

        var jokerRect = jokerButtonObj.GetComponent<RectTransform>();
        var x = jokerStartPosition.x;
        var y = jokerStartPosition.y + SpawnOffset + (availableJokers.Count * (jokerButtonSize.y + distanceBetweenJokers));
        jokerRect.anchoredPosition = new Vector2(x, y);

        var jokerButton = jokerButtonObj.GetComponent<Button>();
        jokerButton.onClick.AddListener(new UnityAction(() => OnJokerClick(jokerButtonObj.gameObject, joker)));

        var jokerImageObj = jokerButtonObj.GetChild(0);
        var jokerImage = jokerImageObj.GetComponent<Image>();
        jokerImage.sprite = joker.Image;

        availableJokers.Add(joker);

        var contentHeight = -jokerStartPosition.y + (availableJokers.Count * jokerButtonSize.y); 
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, contentHeight);
    }

    void OnJokerClick(GameObject jokerObj, IJoker joker)
    {
        joker.Activate();
        availableJokers.Remove(joker);
        Destroy(jokerObj);//TODO: EXIT ANIMATION?
    }
}

