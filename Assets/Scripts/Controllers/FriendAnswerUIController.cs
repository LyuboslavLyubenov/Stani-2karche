using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class FriendAnswerUIController : ExtendedMonoBehaviour
    {
        public GameObject FriendUsername;
        public GameObject FriendAnswer;

        Text usernameText;
        Text answerText;

        string[] offlineFriendNames = { "Гошо", "Пешо", "г-н Кидиков" };

        void Start()
        {
            if (this.FriendUsername == null)
            {
                throw new NullReferenceException("FriendUsername is null on FriendAnswerUIController obj");
            }

            if (this.FriendAnswer == null)
            {
                throw new NullReferenceException("FriendAnswer is null on FriendAnswerUIController obj");
            }

            this.usernameText = this.FriendUsername.GetComponent<Text>();
            this.answerText = this.FriendAnswer.GetComponent<Text>();

            if (this.usernameText == null)
            {
                throw new Exception("FriendUsername must have Text component");
            }

            if (this.answerText == null)
            {
                throw new Exception("FriendAnswer msut have Text component");
            }
        }

        public void SetResponse(string answer)
        {
            var usernameIndex = UnityEngine.Random.Range(0, this.offlineFriendNames.Length);
            var username = this.offlineFriendNames[usernameIndex];

            this.CoroutineUtils.WaitForFrames(0, () => this._SetResponse(username, answer));
        }

        public void SetResponse(string username, string answer)
        {
            this.CoroutineUtils.WaitForFrames(0, () => this._SetResponse(username, answer));
        }

        void _SetResponse(string username, string answer)
        {
            this.usernameText.text = username;
            this.answerText.text = answer;
        }
    }

}
