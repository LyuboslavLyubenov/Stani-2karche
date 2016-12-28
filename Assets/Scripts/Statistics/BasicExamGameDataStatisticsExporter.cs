namespace Assets.Scripts.Statistics
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CSharpJExcel.Jxl;
    using CSharpJExcel.Jxl.Write;
    using System.Collections.Generic;

    using Interfaces;
    using Utils;

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
        IGameDataIterator gameDataIterator;

        public BasicExamGameDataStatisticsExporter(BasicExamStatisticsCollector statisticsCollector, IGameDataIterator gameDataIterator)
        {   
            if (statisticsCollector == null)
            {
                throw new ArgumentNullException("statisticsCollector");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }
            
            this.statisticsCollector = statisticsCollector;
            this.gameDataIterator = gameDataIterator;
        }

        int ExtractCorrectAnsweredQuestionCount(Sheet sheet, int questionRow)
        {
            var correctAnsweredCountCell = sheet.GetCellOrDefault(CorrectAnsweredQuestionCountColumn, questionRow);
            return correctAnsweredCountCell.getContents().ConvertToOrDefault<int>();
        }

        int ExtractIncorrectAnsweredQuestionCount(Sheet sheet, int questionRow)
        {
            var incorrectAnsweredCountCell = sheet.GetCellOrDefault(WrongAnsweredQuestionCountColumn, questionRow);
            return incorrectAnsweredCountCell.getContents().ConvertToOrDefault<int>();
        }

        int ExtractTotalTimeSpentThinking(Sheet sheet, int questionRow)
        {
            var totalTimeSpentCell = sheet.GetCellOrDefault(TotalTimeSpentOnQuestionColumn, questionRow);
            return totalTimeSpentCell.getContents().ConvertToOrDefault<int>();
        }

        float ExtractAvgTimeSpentThinking(Sheet sheet, int questionRow)
        {
            var avgTimeSpentCell = sheet.GetCellOrDefault(AvgSpentTimeOnQuestionColumn, questionRow);
            return avgTimeSpentCell.getContents().ConvertToOrDefault<float>();
        }

        int[] ExtractAnswersSelectedCount(Sheet sheet, int questionRow, int answersCount)
        {
            var answersSelectedCount = new int[answersCount];
            var startRow = questionRow + 1;

            for (int i = 0; i < answersCount; i++)
            {
                var selectedCountCell = sheet.GetCellOrDefault(0, startRow + i);
                var selectedCount = selectedCountCell.getContents().ConvertToOrDefault<int>();
                answersSelectedCount[i] = selectedCount;
            }

            return answersSelectedCount;
        }

        string ExtractCorrectAnswer(Sheet sheet, int questionRow, int answersCount)
        {
            var correctAnswer = string.Empty;
            var startRow = questionRow + 1;

            for (int i = 0; i < answersCount; i++)
            {
                var answerRow = startRow + i;
                var isCorrectAnswerCell = sheet.GetCellOrDefault(IsCorrectAnswerColumn, answerRow);
                var isCorrect = isCorrectAnswerCell.getContents().ToUpperInvariant() == ("верен").ToUpperInvariant();

                if (!isCorrect)
                {
                    continue;
                }
                
                var answerCell = sheet.GetCellOrDefault(0, answerRow);
                correctAnswer = answerCell.getContents();
                break;
            }
            
            return correctAnswer;
        }

        int ExtractAnswerIndex(Sheet sheet, int questionRow, string answer)
        {
            for (int i = questionRow + 1; ; i++)
            {
                Cell answerCell;

                try
                {
                    answerCell = sheet.getCell(0, i);
                }
                catch
                {
                    break;
                }
                
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

        int ExtractQuestionAnswersCount(Sheet sheet, int questionRow)
        {
            for (int i = 1; ; i++)
            {
                var isCellEmpty = !sheet.IsCellEmpty(0, questionRow + i);

                if (isCellEmpty)
                {
                    return i;
                }
            }
        }

        void UpdateQuestionStatisticsData(Sheet sheet, WritableSheet sheetW, int questionRow, int answersCount)
        {
            var questionTextCell = sheet.GetCellOrDefault(0, questionRow);
            var questionText = questionTextCell.getContents();

            if (string.IsNullOrEmpty(questionText))
            {
                throw new Exception("Question text cannot be empty");
            }

            if (this.statisticsCollector.LastQuestion.Text != questionText &&
                this.statisticsCollector.CorrectAnsweredQuestions.All(q => q.Text != questionText))
            {
                return;
            }

            var correctAnsweredCount = this.ExtractCorrectAnsweredQuestionCount(sheet, questionRow);
            var incorrectAnsweredCount = this.ExtractIncorrectAnsweredQuestionCount(sheet, questionRow);
            var totalTimeSpentThinking = this.ExtractTotalTimeSpentThinking(sheet, questionRow);
            var avgTimeSpentThinking = this.ExtractAvgTimeSpentThinking(sheet, questionRow);
            var answersSelectedCount = this.ExtractAnswersSelectedCount(sheet, questionRow, answersCount);
            var usedJokersCount = this.ExtractUsedJokersCount(sheet, questionRow);
            var correctAnswer = this.ExtractCorrectAnswer(sheet, questionRow, answersCount);
            var correctAnswerIndex = this.ExtractAnswerIndex(sheet, questionRow, correctAnswer);

            var currentGameQuestionSpentTime = this.statisticsCollector.QuestionsSpentTime
                .First(q => q.Key.Text == questionText)
                .Value;
        
            totalTimeSpentThinking += currentGameQuestionSpentTime;
            avgTimeSpentThinking += currentGameQuestionSpentTime;

            if ((avgTimeSpentThinking - currentGameQuestionSpentTime) >= 0.1f)
            {
                avgTimeSpentThinking /= 2;
            }

            if (this.statisticsCollector.LastQuestion.Text != questionText)
            {
                answersSelectedCount[correctAnswerIndex]++;
                correctAnsweredCount++;
            }
            else
            {
                var incorrectAnswerIndex = this.ExtractAnswerIndex(sheet, questionRow, this.statisticsCollector.LastSelectedAnswer);
                answersSelectedCount[incorrectAnswerIndex]++;
                incorrectAnsweredCount++;
            }

            try
            {
                usedJokersCount += this.statisticsCollector.QuestionsUsedJokers
                    .First(q => q.Key.Text == questionText).Value.Count;
            }
            catch
            {
            }

            sheetW.addCell(new Number(CorrectAnsweredQuestionCountColumn, questionRow, correctAnsweredCount));
            sheetW.addCell(new Number(WrongAnsweredQuestionCountColumn, questionRow, incorrectAnsweredCount));
            sheetW.addCell(new Number(TotalTimeSpentOnQuestionColumn, questionRow, totalTimeSpentThinking));
            sheetW.addCell(new Number(AvgSpentTimeOnQuestionColumn, questionRow, avgTimeSpentThinking));

            for (int i = 0; i < answersSelectedCount.Length; i++)
            {
                var answerSelectedCount = answersSelectedCount[i];
                var answerRow = questionRow + i + 1;
                sheetW.addCell(new Number(SelectedAnswerCountOnQuestionColumn, answerRow, answerSelectedCount));
            }

            sheetW.addCell(new Number(UsedJokersCountOnQuestionColumn, questionRow, usedJokersCount));
        }

        public void Export()
        {
            for (int mark = GameDataIterator.MarkMin; ; mark++)
            {
                var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..";
                var path = string.Format("{0}\\{1}{2}\\{3}.xls", execPath, GameDataExtractor.LevelPath, this.gameDataIterator.LevelCategory, mark);

                if (!File.Exists(path))
                {
                    break;
                }

                var newPath = path + ".new";
                
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                File.Copy(path, newPath);

                var fileInfo = new FileInfo(path);
                var workbook = Workbook.getWorkbook(fileInfo);
                var sheet = workbook.getSheet(0);

                var workbookW = Workbook.createWorkbook(new FileInfo(newPath), workbook);
                var sheetW = workbookW.getSheet(0);
                
                for (int i = GameDataExtractor.QuestionsStartRow; i < sheet.getRows(); )
                {
                    var questionAnswersCount = ExtractQuestionAnswersCount(sheet, i);
                    this.UpdateQuestionStatisticsData(sheet, sheetW, i, questionAnswersCount);
                    i += questionAnswersCount + 2; //1 question text row, 1 empty row 
                }

                workbookW.write();
                workbookW.close();
                workbook.close();

                File.Delete(path);
                File.Move(newPath, path);
            }
        }
    }

}
