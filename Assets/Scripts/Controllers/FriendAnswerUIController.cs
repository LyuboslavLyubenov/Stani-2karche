namespace Controllers
{

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class FriendAnswerUIController : ExtendedMonoBehaviour
    {
        public GameObject FriendUsername;
        public GameObject FriendAnswer;

        private Text usernameText;

        private Text answerText;

        private string[] offlineFriendNames = {
                                                    "Георги Кидиков", "100 кила",
                                                    "Гери-Никол", "Wiz Khalifa",
                                                    "Napoleon Hill", "Daniel Kholeman",
                                                    "Иван Вазов", "Хари Потър",
                                                    "Бойко Борисов", "Асен Златаров",
                                                    "Адолф Хитлер", "Йосиф Сталин",
                                                    "Доналт Тръмп", "Мики Маус"
                                                };

        public string Username
        {
            get
            {
                return this.usernameText.text;
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException();
                }

                this.usernameText.text = value;
            }
        }

        public string Answer
        {
            get
            {
                return this.answerText.text;
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException();
                }

                this.answerText.text = value;
            }
        }

        void Awake()
        {
            this.usernameText = this.FriendUsername.GetComponent<Text>();
            this.answerText = this.FriendAnswer.GetComponent<Text>();
        }

        private void _SetResponse(string username, string answer)
        {
            this.usernameText.text = username;
            this.answerText.text = answer;
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
    }

    public class PlayerAnswerResponse
    {
        public string Username
        {
            get; set;
        }

        public string Answer
        {
            get; set;
        }
    }
}
