namespace Assets.Scripts.Controllers
{
    using UnityEngine;

    using Network.Servers;

    using EventArgs;

    using EventArgs = System.EventArgs;

    public class GameDataInfoUIController : MonoBehaviour
    {
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
            
            this.Server.GameDataIterator.OnLoaded += this.OnGameDataLoaded;
            this.Server.GameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.Server.GameDataQuestionsSender.OnSentQuestion += this.OnSentQuestion;
        }

        private void OnGameDataLoaded(object sender, EventArgs args)
        {
            this.categoryField.Value = this.Server.GameDataIterator.LevelCategory;
            this.currentMarkField.Value = this.Server.GameDataIterator.CurrentMark.ToString();
            this.remainingQuestionsField.Value = this.Server.GameDataIterator.RemainingQuestionsToNextMark.ToString();
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.currentMarkField.Value = this.Server.GameDataIterator.CurrentMark.ToString();
        }

        private void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            this.remainingQuestionsField.Value = this.Server.GameDataIterator.RemainingQuestionsToNextMark.ToString();
            this.currentQuestionField.Value = args.Question.Text;
        }
    }
}