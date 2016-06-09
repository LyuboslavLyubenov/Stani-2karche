using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Question
{
    public string Text;
    public List<string> Answers;
    public int CorrectAnswerIndex;
}
