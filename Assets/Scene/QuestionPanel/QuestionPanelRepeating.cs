using Controllers;
using Assets.Tests.Utils;

public class QuestionPanelRepeating : QuestionUIController
{
    private QuestionGenerator questionGenerator = new QuestionGenerator();

	// Use this for initialization
	void Awake () 
    {
        var question = this.questionGenerator.GenerateQuestion();
        base.LoadQuestion(question);

        base.OnAnswerClick += (sender, args) =>
        {
                base.CoroutineUtils.WaitForSeconds(1, this.LoadNewQuestion);
        };
	}

    private void LoadNewQuestion()
    {
        base.LoadQuestion(questionGenerator.GenerateQuestion());
    }	
}
