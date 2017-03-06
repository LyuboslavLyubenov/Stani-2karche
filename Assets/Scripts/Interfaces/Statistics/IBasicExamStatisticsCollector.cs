namespace Assets.Scripts.Interfaces.Statistics
{

    using System;
    using System.Collections.Generic;

    public interface IBasicExamStatisticsCollector
    {
        IDictionary<ISimpleQuestion, List<Type>> QuestionsUsedJokers
        {
            get;
        }

        IDictionary<Type, int> JokersUsedTimes
        {
            get;
        }

        IDictionary<ISimpleQuestion, int> QuestionsSpentTime
        {
            get;
        }

        IList<ISimpleQuestion> CorrectAnsweredQuestions
        {
            get;
        }

        ISimpleQuestion LastQuestion
        {
            get;
        }

        string LastSelectedAnswer
        {
            get;
        }

        int EndMark
        {
            get;
        }

        int PlayerScore
        {
            get;
        }
    }
}
