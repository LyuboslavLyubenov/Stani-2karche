namespace Interfaces.GameData
{

    using System;

    using DTOs;

    public interface IGameDataExtractor
    {
        event EventHandler OnLoaded;

        bool ShuffleQuestions { get; set; }

        bool ShuffleAnswers { get; set; }

        string LevelCategory { get; set; }

        bool Loaded { get; }

        bool Loading { get; }

        int MaxMarkIndex { get; }

        void ExtractDataAsync(Action<Exception> onError);

        void ExtractDataSync();

        ExtractedQuestion GetQuestion(int markIndex, int questionIndex);

        int GetQuestionsCountForMark(int markIndex);
    }
}
