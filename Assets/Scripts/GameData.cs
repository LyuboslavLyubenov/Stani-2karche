using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GameData : MonoBehaviour
{
    LevelData levelData = null;
    int questionIndex = 0;
    int currentMark = 2;

    public EventHandler<MarkEventArgs> MarkIncrease = delegate
    {
    };

    void Start()
    {
        LoadLevelData();
    }

    void LoadLevelData()
    {
        var levelDataJson = File.ReadAllText("leveldata.txt");
        levelData = JsonUtility.FromJson<LevelData>(levelDataJson);
    }

    public Question GetCurrentQuestion()
    {
        var index = Mathf.Min(levelData.Questions.Count - 1, questionIndex - 1);
        return levelData.Questions[index];
    }

    public Question GetNextQuestion()
    {
        var mark = (((questionIndex) * 6) / levelData.Questions.Count);
        var nextMark = Mathf.Min(6, ((questionIndex + 1) * 6) / levelData.Questions.Count);

        if (mark > currentMark)
        {
            MarkIncrease(this, new MarkEventArgs(mark, nextMark));
            currentMark = mark;
        }

        if (questionIndex >= levelData.Questions.Count)
        {
            return null;   
        }
        else
        {
            var question = levelData.Questions[questionIndex++];
            return question;
        }
    }
}

