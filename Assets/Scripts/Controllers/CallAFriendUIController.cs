using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Call A friend user interface controller.
/// </summary>
public class CallAFriendUIController : MonoBehaviour
{
    public Button NextButton;
    public Button PrevButton;

    public GameObject FriendsToSelectPanel;

    public EventHandler<PlayerCalledEventArgs> OnCalledPlayer = delegate
    {
    };

    Button[] friendSelectToCallButtons = null;
    Text[] friendSelectToCallNames = null;

    int currentPageIndex = 0;
    //pages on the phone
    List<CallFriendPageElement[]> pages = new List<CallFriendPageElement[]>();

    void Start()
    {
        friendSelectToCallButtons = FriendsToSelectPanel.GetComponentsInChildren<Button>();
        friendSelectToCallNames = new Text[friendSelectToCallButtons.Length];

        for (int i = 0; i < friendSelectToCallNames.Length; i++)
        {
            friendSelectToCallNames[i] = friendSelectToCallButtons[i].GetComponentInChildren<Text>();
        }

        PrevButton.onClick.AddListener(OnPreviousButtonClick);
        NextButton.onClick.AddListener(OnNextButtonClick);
    }

    void OnDisable()
    {
        DisableAllButtons();
    }

    void OnNextButtonClick()
    {
        currentPageIndex++;
        ReloadContacts();

        if (currentPageIndex < pages.Count - 1)
        {
            NextButton.interactable = true;
        }
    }

    void OnPreviousButtonClick()
    {
        currentPageIndex--;
        ReloadContacts();

        if (currentPageIndex > 0)
        {
            PrevButton.interactable = true;
        }
    }

    void DisableAllButtons()
    {
        for (int i = 0; i < friendSelectToCallButtons.Length; i++)
        {
            friendSelectToCallButtons[i].gameObject.SetActive(false);
        }
    }

    void ReloadContacts()
    {
        if (pages.Count <= 0)
        {
            return;
        }

        var page = pages[currentPageIndex];

        DisableAllButtons();

        for (int i = 0; i < pages[currentPageIndex].Length; i++)
        {
            //reload
            var friendData = pages[currentPageIndex][i];
            var friendConnectionId = friendData.ConnectionId;
            var eventArgs = new PlayerCalledEventArgs(friendConnectionId);

            friendSelectToCallButtons[i].gameObject.SetActive(true);
            friendSelectToCallButtons[i].onClick.RemoveAllListeners();
            friendSelectToCallButtons[i].onClick.AddListener(() => OnCalledPlayer(this, eventArgs));
            friendSelectToCallNames[i].text = friendData.Name; 
        }

        if (currentPageIndex >= pages.Count - 1)
        {
            NextButton.interactable = false;
        }

        if (currentPageIndex <= 0)
        {
            PrevButton.interactable = false;
        }
    }

    public void SetContacts(Dictionary<int, string> playersConnectionIdName)
    {
        StartCoroutine(SetContactsCoroutine(playersConnectionIdName));
    }

    IEnumerator SetContactsCoroutine(Dictionary<int, string> playersConnectionIdName)
    {
        yield return null;

        currentPageIndex = 0;
        pages.Clear();

        for (int i = 0; i < playersConnectionIdName.Count; i += 4)
        {
            var page = playersConnectionIdName.Skip(i)
                .Take(4)
                .Select(d => new CallFriendPageElement(d.Key, d.Value))
                .ToArray();

            pages.Add(page);
        }

        ReloadContacts();
    }

}