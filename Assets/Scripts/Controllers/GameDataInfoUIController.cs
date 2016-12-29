namespace Assets.Scripts.Controllers
{
    using UnityEngine;

    using IO;
    using Network.Servers;

    using EventArgs;
    using Network;

    using EventArgs = System.EventArgs;

    public class GameDataInfoUIController : MonoBehaviour
    {
        public GameDataIterator GameData;
        public GameDataSender GameDataSender;
        public BasicExamServer Server;

        private FieldUIController categoryField;

        private FieldUIController currentQuestionField;

        private FieldUIController currentMarkField;

        private FieldUIController remainingQuestionsField;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.categoryField = this.transform.Find("CategoryField").GetComponent<FieldUIController>();
            this.currentQuestionField = this.transform.Find("CurrentQuestionField").GetComponent<FieldUIController>();
            this.currentMarkField = this.transform.Find("CurrentMarkField").GetComponent<FieldUIController>();
            this.remainingQuestionsField = this.transform.Find("RemainingQuestionsField").GetComponent<FieldUIController>();

            this.GameData.OnLoaded += this.OnGameDataLoaded;
            this.GameData.OnMarkIncrease += this.OnMarkIncrease;
            this.GameDataSender.OnSentQuestion += this.OnSentQuestion;
        }

        private void OnGameDataLoaded(object sender, EventArgs args)
        {
            this.categoryField.Value = this.GameData.LevelCategory;
            this.currentMarkField.Value = this.GameData.CurrentMark.ToString();
            this.remainingQuestionsField.Value = this.GameData.RemainingQuestionsToNextMark.ToString();
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.currentMarkField.Value = this.GameData.CurrentMark.ToString();
        }

        private void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            this.remainingQuestionsField.Value = this.GameData.RemainingQuestionsToNextMark.ToString();
            this.currentQuestionField.Value = args.Question.Text;
        }
    }

}
