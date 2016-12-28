using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    /// <summary>
    /// Call A friend user interface controller.
    /// </summary>
    public class CallAFriendUIController : ExtendedMonoBehaviour
    {
        public Button NextButton;
        public Button PrevButton;

        public GameObject FriendsToSelectPanel;

        public Text PagePositionText;

        public EventHandler<PlayerCalledEventArgs> OnCalledPlayer = delegate
            {
            };

        Button[] friendSelectToCallButtons = null;
        Text[] friendSelectToCallNames = null;

        int currentPageIndex = 0;
        //pages on the phone
        List<CallFriendPageElement[]> pages = new List<CallFriendPageElement[]>();

        void Awake()
        {
            this.friendSelectToCallButtons = this.FriendsToSelectPanel.GetComponentsInChildren<Button>();
            this.friendSelectToCallNames = new Text[this.friendSelectToCallButtons.Length];

            for (int i = 0; i < this.friendSelectToCallNames.Length; i++)
            {
                this.friendSelectToCallNames[i] = this.friendSelectToCallButtons[i].GetComponentInChildren<Text>();
            }

            this.PrevButton.onClick.AddListener(this.OnPreviousButtonClick);
            this.NextButton.onClick.AddListener(this.OnNextButtonClick);
        }

        void OnNextButtonClick()
        {
            this.currentPageIndex++;
            this.ReloadContacts();

            if (this.currentPageIndex < this.pages.Count - 1)
            {
                this.NextButton.interactable = true;
            }
        }

        void OnPreviousButtonClick()
        {
            this.currentPageIndex--;
            this.ReloadContacts();

            if (this.currentPageIndex > 0)
            {
                this.PrevButton.interactable = true;
            }
        }

        void ReloadContacts()
        {
            this.RenderContactNames();

            this.PagePositionText.text = string.Format("{0}/{1}", this.currentPageIndex, this.pages.Count);

            if (this.currentPageIndex >= this.pages.Count - 1)
            {
                this.NextButton.interactable = false;
            }

            if (this.currentPageIndex <= 0)
            {
                this.PrevButton.interactable = false;
            }
        }

        void RenderContactNames()
        {
            var page = this.pages[this.currentPageIndex];

            for (int i = 0; i < page.Length; i++)
            {
                //reload
                var friendData = page[i];
                var friendConnectionId = friendData.ConnectionId;
                var eventArgs = new PlayerCalledEventArgs(friendConnectionId);

                this.friendSelectToCallButtons[i].gameObject.SetActive(true);
                this.friendSelectToCallButtons[i].onClick.RemoveAllListeners();
                this.friendSelectToCallButtons[i].onClick.AddListener(() => this.OnCalledPlayer(this, eventArgs));
                this.friendSelectToCallNames[i].text = friendData.Name;
            }
        }

        public void SetContacts(Dictionary<int, string> playersConnectionIdName)
        {
            this.currentPageIndex = 0;
            this.pages.Clear();

            for (int i = 0; i < playersConnectionIdName.Count; i += 4)
            {
                var page = playersConnectionIdName.Skip(i)
                    .Take(4)
                    .Select(d => new CallFriendPageElement(d.Key, d.Value))
                    .ToArray();

                this.pages.Add(page);
            }

            this.ReloadContacts();
        }

    }

}