using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;

public class AvailableJokersUIController : MonoBehaviour
{
    const int SpawnOffset = 10;

    public EventHandler<JokerEventArgs> OnAddedJoker = delegate
    {
    };

    public EventHandler<JokerEventArgs> OnUsedJoker = delegate
    {
    };

    public Transform Container;

    RectTransform containerRectTransform;

    List<IJoker> jokers = new List<IJoker>();
    Transform dummyJokerButtonPrefab;
    Vector2 jokerStartPosition;
    Vector2 jokerButtonSize;

    float distanceBetweenJokers = 0;

    public IJoker[] Jokers
    {
        get
        {
            return jokers.ToArray();
        }
    }

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
        if (joker == null)
        {
            throw new ArgumentNullException("joker");
        }
            
        var jokerButtonObj = Instantiate(dummyJokerButtonPrefab);

        jokerButtonObj.SetParent(Container, false);

        var jokerRect = jokerButtonObj.GetComponent<RectTransform>();
        var x = jokerStartPosition.x;
        var y = jokerStartPosition.y + SpawnOffset + (jokers.Count * (jokerButtonSize.y + distanceBetweenJokers));
        jokerRect.anchoredPosition = new Vector2(x, y);

        var jokerButton = jokerButtonObj.GetComponent<Button>();
        jokerButton.onClick.AddListener(new UnityAction(() => OnJokerClick(jokerButtonObj.gameObject, joker)));

        var jokerImageObj = jokerButtonObj.GetChild(0);
        var jokerImage = jokerImageObj.GetComponent<Image>();
        jokerImage.sprite = joker.Image;

        jokers.Add(joker);

        var contentHeight = -jokerStartPosition.y + (jokers.Count * jokerButtonSize.y); 
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, contentHeight);

        OnAddedJoker(this, new JokerEventArgs(joker));
    }

    void OnJokerClick(GameObject jokerObj, IJoker joker)
    {
        try
        {
            joker.Activate();  
        }
        catch (InvalidOperationException ex)
        {
            Debug.LogException(ex);
        }

        OnUsedJoker(this, new JokerEventArgs(joker));

        jokers.Remove(joker);
        Destroy(jokerObj);//TODO: EXIT ANIMATION?  
    }
}