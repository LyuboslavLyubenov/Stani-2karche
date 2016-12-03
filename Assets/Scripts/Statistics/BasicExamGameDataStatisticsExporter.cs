using CSharpJExcel.Jxl;
using System.IO;
using System.Linq;
using CSharpJExcel.Jxl.Write;
using System;
using System.Reflection;

public class BasicExamGameDataStatisticsExporter : IStatisticsExporter
{
    const int IsCorrectAnswerColumn = 1;
    const int CorrectAnsweredQuestionCountColumn = 2;
    const int WrongAnsweredQuestionCountColumn = 3;
    const int TotalTimeSpentOnQuestionColumn = 4;
    const int AvgSpentTimeOnQuestionColumn = 5;
    const int SelectedAnswerCountOnQuestionColumn = 6;
    const int UsedJokersCountOnQuestionColumn = 7;

    BasicExamStatisticsCollector statisticsCollector;
    IGameData gameData;

    public BasicExamGameDataStatisticsExporter(BasicExamStatisticsCollector statisticsCollector, IGameData gameData)
    {   
        if (statisticsCollector == null)
        {
            throw new System.ArgumentNullException("statisticsCollector");
        }

        if (gameData == null)
        {
            throw new System.ArgumentNullException("gameData");
        }
            
        this.statisticsCollector = statisticsCollector;
        this.gameData = gameData;
    }

    int ExtractCorrectAnsweredQuestionCount(Sheet sheet, int row)
    {
        var correctAnsweredCountCell = sheet.GetCellOrDefault(CorrectAnsweredQuestionCountColumn, row);
        return correctAnsweredCountCell.getContents().ConvertToOrDefault<int>();
    }

    int ExtractIncorrectAnsweredQuestionCount(Sheet sheet, int row)
    {
        var incorrectAnsweredCountCell = sheet.GetCellOrDefault(WrongAnsweredQuestionCountColumn, row);
        return incorrectAnsweredCountCell.getContents().ConvertToOrDefault<int>();
    }

    int ExtractTotalTimeSpentThinking(Sheet sheet, int row)
    {
        var totalTimeSpentCell = sheet.GetCellOrDefault(TotalTimeSpentOnQuestionColumn, row);
        return totalTimeSpentCell.getContents().ConvertToOrDefault<int>();
    }

    float ExtractAvgTimeSpentThinking(Sheet sheet, int row)
    {
        var avgTimeSpentCell = sheet.GetCellOrDefault(AvgSpentTimeOnQuestionColumn, row);
        return avgTimeSpentCell.getContents().ConvertToOrDefault<float>();
    }

    int[] ExtractSelectedAnswerCount(Sheet sheet, int row)
    {
        var answersSelectedCount = new int[4];
        var startRow = row + 1;

        for (int i = 0; i < 4; i++)
        {
            var selectedCountCell = sheet.GetCellOrDefault(SelectedAnswerCountOnQuestionColumn, startRow + i);
            var selectedCount = selectedCountCell.getContents().ConvertToOrDefault<int>();
            answersSelectedCount[i] = selectedCount;
        }

        return answersSelectedCount;
    }

    string ExtractCorrectAnswer(Sheet sheet, int row)
    {
        var correctAnswer = string.Empty;

        for (int i = 0; i < 4; i++)
        {
            var answerRow = row + i + 1;
            var isCorrectAnswerCell = sheet.GetCellOrDefault(IsCorrectAnswerColumn, answerRow);
            var isCorrect = isCorrectAnswerCell.getContents().ToUpperInvariant() == "Верен".ToUpperInvariant();

            if (isCorrect)
            {
                var answerCell = sheet.GetCellOrDefault(0, answerRow);
                correctAnswer = answerCell.getContents();
                break;
            }
        }

        return correctAnswer;
    }

    int ExtractAnswerIndex(Sheet sheet, int row, string answer)
    {
        for (int i = 0; i < 4; i++)
        {
            var answerRow = row + i + 1;
            var answerCell = sheet.GetCellOrDefault(0, answerRow);

            if (answerCell.getContents() == answer)
            {
                return i;
            }
        }  

        return -1;
    }

    int ExtractUsedJokersCount(Sheet sheet, int questionRow)
    {
        var usedJokersCountCell = sheet.GetCellOrDefault(UsedJokersCountOnQuestionColumn, questionRow);
        return usedJokersCountCell.getContents().ConvertToOrDefault<int>();
    }

    void UpdateQuestionData(Sheet sheet, int questionRow, WritableWorkbook workbookW, WritableSheet sheetW)
    {
        var questionTextCell = sheet.GetCellOrDefault(0, questionRow);
        var questionText = questionTextCell.getContents();

        if (string.IsNullOrEmpty(questionText))
        {
            throw new Exception("Question text cannot be empty");
        }

        if (statisticsCollector.LastQuestion.Text != questionText &&
            statisticsCollector.CorrectAnsweredQuestions.All(q => q.Text != questionText))
        {
            return;
        }

        var correctAnsweredCount = ExtractCorrectAnsweredQuestionCount(sheet, questionRow);
        var incorrectAnsweredCount = ExtractIncorrectAnsweredQuestionCount(sheet, questionRow);
        var totalTimeSpentThinking = ExtractTotalTimeSpentThinking(sheet, questionRow);
        var avgTimeSpentThinking = ExtractAvgTimeSpentThinking(sheet, questionRow);
        var selectedAnswersCount = ExtractSelectedAnswerCount(sheet, questionRow);
        var usedJokersCount = ExtractUsedJokersCount(sheet, questionRow);
        var correctAnswer = ExtractCorrectAnswer(sheet, questionRow);
        var correctAnswerIndex = ExtractAnswerIndex(sheet, questionRow, correctAnswer);

        var currentGameQuestionSpentTime = statisticsCollector.QuestionsSpentTime
            .First(q => q.Key.Text == questionText)
            .Value;
        
        totalTimeSpentThinking += currentGameQuestionSpentTime;
        avgTimeSpentThinking += currentGameQuestionSpentTime;

        if ((avgTimeSpentThinking - currentGameQuestionSpentTime) >= 0.1f)
        {
            avgTimeSpentThinking /= 2;
        }

        if (statisticsCollector.LastQuestion.Text != questionText)
        {
            selectedAnswersCount[correctAnswerIndex]++;
            correctAnsweredCount++;
        }
        else
        {
            var incorrectAnswerIndex = ExtractAnswerIndex(sheet, questionRow, statisticsCollector.LastSelectedAnswer);
            selectedAnswersCount[incorrectAnswerIndex]++;
            incorrectAnsweredCount++;
        }

        try
        {
            usedJokersCount += statisticsCollector.QuestionsUsedJokers
                .First(q => q.Key.Text == questionText).Value.Count;
        }
        catch (System.Exception ex)
        {
            
        }

        sheetW.addCell(new Number(CorrectAnsweredQuestionCountColumn, questionRow, correctAnsweredCount));
        sheetW.addCell(new Number(WrongAnsweredQuestionCountColumn, questionRow, incorrectAnsweredCount));
        sheetW.addCell(new Number(TotalTimeSpentOnQuestionColumn, questionRow, totalTimeSpentThinking));
        sheetW.addCell(new Number(AvgSpentTimeOnQuestionColumn, questionRow, avgTimeSpentThinking));

        for (int i = 0; i < 4; i++)
        {
            var selectedAnswerCount = selectedAnswersCount[i];
            var answerRow = questionRow + i + 1;
            sheetW.addCell(new Number(SelectedAnswerCountOnQuestionColumn, answerRow, selectedAnswerCount));
        }

        sheetW.addCell(new Number(UsedJokersCountOnQuestionColumn, questionRow, usedJokersCount));
    }

    public void Export()
    {
        for (int mark = LocalGameData.MarkMin; mark <= LocalGameData.MarkMax; mark++)
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..";
            var path = string.Format("{0}\\{1}{2}\\{3}.xls", execPath, LocalGameData.LevelPath, gameData.LevelCategory, mark);
            var newPath = path + ".new";

            File.Copy(path, newPath);

            var fileInfo = new FileInfo(path);
            var workbook = Workbook.getWorkbook(fileInfo);
            var sheet = workbook.getSheet(0);

            var workbookW = Workbook.createWorkbook(new FileInfo(newPath), workbook);
            var sheetW = workbookW.getSheet(0);

            for (int i = LocalGameData.SettingsStartPosition.Row; i < sheet.getRows() - 6; i += 6)
            {
                UpdateQuestionData(sheet, i, workbookW, sheetW);
            }

            workbookW.write();
            workbookW.close();
            workbook.close();

            File.Delete(path);
            File.Move(newPath, path);
        }
    }
}
