using UnityEngine;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.IO;

    using EventArgs;
    using Network;

    using EventArgs = System.EventArgs;

    public class GameDataInfoUIController : MonoBehaviour
    {
        public GameDataIterator GameData;
        public GameDataSender GameDataSender;
        public BasicExamServer Server;

        FieldUIController categoryField;
        FieldUIController currentQuestionField;
        FieldUIController currentMarkField;
        FieldUIController remainingQuestionsField;

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

        void OnGameDataLoaded(object sender, EventArgs args)
        {
            this.categoryField.Value = this.GameData.LevelCategory;
            this.currentMarkField.Value = this.GameData.CurrentMark.ToString();
            this.remainingQuestionsField.Value = this.GameData.RemainingQuestionsToNextMark.ToString();
        }

        void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.currentMarkField.Value = this.GameData.CurrentMark.ToString();
        }

        void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            this.remainingQuestionsField.Value = this.GameData.RemainingQuestionsToNextMark.ToString();
            this.currentQuestionField.Value = args.Question.Text;
        }
    }

}
