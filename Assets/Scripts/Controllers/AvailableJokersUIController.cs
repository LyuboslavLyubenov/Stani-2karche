﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

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

    public int JokersCount
    {
        get
        {
            return jokerObjs.Count;
        }
    }

    RectTransform containerRectTransform;

    List<Transform> jokerObjs = new List<Transform>();

    List<IJoker> jokers = new List<IJoker>();
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

    public void AddJoker(IJoker joker)
    {
        if (joker == null)
        {
            throw new ArgumentNullException("joker");
        }
            
        var jokerObj = Instantiate(dummyJokerButtonPrefab);

        jokerObj.name = joker.GetType().Name.Replace("Joker", "");
        jokerObj.SetParent(Container, false);

        var jokerRect = jokerObj.GetComponent<RectTransform>();
        var x = jokerStartPosition.x;
        var y = jokerStartPosition.y + SpawnOffset + (jokers.Count * (jokerButtonSize.y + distanceBetweenJokers));

        jokerRect.anchoredPosition = new Vector2(x, y);

        var jokerButton = jokerObj.GetComponent<Button>();
        jokerButton.onClick.AddListener(new UnityAction(() => OnJokerClick(jokerObj.gameObject, joker)));

        var jokerImageObj = jokerObj.GetChild(0);
        var jokerImage = jokerImageObj.GetComponent<Image>();
        jokerImage.sprite = joker.Image;

        jokerObjs.Add(jokerObj);
        jokers.Add(joker);

        var contentHeight = -jokerStartPosition.y + (jokers.Count * (jokerButtonSize.y + 10)); 
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, contentHeight);

        OnAddedJoker(this, new JokerEventArgs(joker));
    }

    public void ClearAll()
    {
        for (int i = 0; i < jokerObjs.Count; i++)
        {
            var jokerObj = jokerObjs[i];

            if (jokerObj == null)
            {
                continue;
            }

            Destroy(jokerObj.gameObject);
        }

        jokers.Clear();
        jokerObjs.Clear();
    }
}