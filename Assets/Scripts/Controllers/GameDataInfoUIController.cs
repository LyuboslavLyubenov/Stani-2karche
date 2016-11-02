using UnityEngine;
using System;

public class GameDataInfoUIController : MonoBehaviour
{
    public LocalGameData GameData;
    public GameDataSender GameDataSender;
    public BasicExamServer Server;

    FieldUIController categoryField;
    FieldUIController currentQuestionField;
    FieldUIController currentMarkField;
    FieldUIController remainingQuestionsField;

    void Start()
    {
        categoryField = transform.Find("CategoryField").GetComponent<FieldUIController>();
        currentQuestionField = transform.Find("CurrentQuestionField").GetComponent<FieldUIController>();
        currentMarkField = transform.Find("CurrentMarkField").GetComponent<FieldUIController>();
        remainingQuestionsField = transform.Find("RemainingQuestionsField").GetComponent<FieldUIController>();

        GameData.OnLoaded += OnGameDataLoaded;
        GameData.OnMarkIncrease += OnMarkIncrease;
        GameDataSender.OnSentQuestion += OnSentQuestion;
    }

    void OnGameDataLoaded(object sender, EventArgs args)
    {
        categoryField.Value = GameData.LevelCategory;
        currentMarkField.Value = GameData.CurrentMark.ToString();
        remainingQuestionsField.Value = GameData.RemainingQuestionsToNextMark.ToString();
    }

    void OnMarkIncrease(object sender, MarkEventArgs args)
    {
        currentMarkField.Value = GameData.CurrentMark.ToString();
    }

    void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
    {
        if (args.QuestionType == QuestionRequestType.Random)
        {
            return;
        }    

        remainingQuestionsField.Value = GameData.RemainingQuestionsToNextMark.ToString();
        currentQuestionField.Value = args.Question.Text;
    }
}
