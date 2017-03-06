namespace Assets.Scripts.Statistics
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Assets.Scripts.Interfaces.Statistics;

    using CSharpJExcel.Jxl;
    using CSharpJExcel.Jxl.Write;

    using UnityEngine;
    using Interfaces;

    public class BasicExamGeneralStatiticsExporter : IStatisticsExporter
    {
        private const string Directory = "Statistics\\";
        private const string FileName = "General";
        private const string Extension = ".xls";

        private readonly string Path;

        private readonly JExcelCellPosition AvgSpentTimeLabelAddress = new JExcelCellPosition(0, 1);
        private readonly JExcelCellPosition UsedJokersCountLabelAddress = new JExcelCellPosition(1, 1);
        private readonly JExcelCellPosition SurrenderCountLabelAddress = new JExcelCellPosition(2, 1);

        private IBasicExamStatisticsCollector statisticsCollector;

        public BasicExamGeneralStatiticsExporter(IBasicExamStatisticsCollector statisticsCollector)
        {
            if (statisticsCollector == null)
            {
                throw new ArgumentNullException("statisticsCollector");
            }
            
            this.statisticsCollector = statisticsCollector;

            var execPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\";
            this.Path = execPath + Directory + FileName + Extension;
        }

        private float ExtractAvgSpentTimeOnQuestion(Sheet sheet)
        {
            var avgSpentTimeCell = sheet.GetCellOrDefault(this.AvgSpentTimeLabelAddress.Column, this.AvgSpentTimeLabelAddress.Row + 1); 
            var avgSpentTime = avgSpentTimeCell.getContents();

            if (string.IsNullOrEmpty(avgSpentTime))
            {
                return 0f;
            }

            return float.Parse(avgSpentTime);
        }

        private float GetAvgSpentTimeOnQuestion()
        {
            var questionSpentTime = this.statisticsCollector.QuestionsSpentTime;
            var sumTime = (float)questionSpentTime.Values.Sum(t => t);
            var avgSpentTime = sumTime / (float)questionSpentTime.Count;
            return avgSpentTime;
        }

        private float CalculateNewAvgSpentTimeOnQuestion(Sheet sheet)
        {
            var currentGameAvgSpentTimeOnQestion = this.GetAvgSpentTimeOnQuestion();
            var avgSpentTimeOnQuestion = this.ExtractAvgSpentTimeOnQuestion(sheet);
            avgSpentTimeOnQuestion += currentGameAvgSpentTimeOnQestion;
            avgSpentTimeOnQuestion /= 2;
            return avgSpentTimeOnQuestion;
        }

        private int ExtractUsedJokerCount(Sheet sheet)
        {
            var usedJokerCountCell = sheet.GetCellOrDefault(this.UsedJokersCountLabelAddress.Column, this.UsedJokersCountLabelAddress.Row + 1);  
            var usedJokerCount = usedJokerCountCell.getContents();

            if (string.IsNullOrEmpty(usedJokerCount))
            {
                return 0;
            }

            return int.Parse(usedJokerCount);
        }

        private int SumUsedJokersCount()
        {
            var jokersUsedTimes = this.statisticsCollector.JokersUsedTimes;
            return jokersUsedTimes.Sum(jut => jut.Value);
        }

        private int CalculateNewUsedJokerCount(Sheet sheet)
        {
            var usedJokersCount = this.ExtractUsedJokerCount(sheet);
            usedJokersCount += this.SumUsedJokersCount();
            return usedJokersCount;
        }

        private int ExtractSurrenderCount(Sheet sheet)
        {
            var surrenderCountCell = sheet.GetCellOrDefault(this.SurrenderCountLabelAddress.Column, this.SurrenderCountLabelAddress.Row + 1);
            var surrenderCount = surrenderCountCell.getContents();

            if (string.IsNullOrEmpty(surrenderCount))
            {
                return 0;
            }

            return int.Parse(surrenderCount);
        }

        public void Export()
        {
            var fileInfo = new FileInfo(this.Path);

            if (!fileInfo.Exists)
            {
                var newWorkbook = WorkbookUtils.CreateEmptyWorkbook(fileInfo, "General");
                newWorkbook.close();
            }

            var workbook = Workbook.getWorkbook(fileInfo);
            var sheet = workbook.getSheet(0);
            var newAvgSpentTime = this.CalculateNewAvgSpentTimeOnQuestion(sheet);
            var newUsedJokerCount = this.CalculateNewUsedJokerCount(sheet);
            var newSurrenderCount = this.ExtractSurrenderCount(sheet);

            if (PlayerPrefs.HasKey("Surrender"))
            {
                newSurrenderCount++;
                PlayerPrefs.DeleteKey("Surrender");//TODO remove?
            }

            var workbookW = Workbook.createWorkbook(fileInfo, workbook);
            var sheetW = workbookW.getSheet(0);

            sheetW.addCell(new Label(this.AvgSpentTimeLabelAddress.Column, this.AvgSpentTimeLabelAddress.Row, "Средно време прекарано на въпрос (в секунди)"));
            sheetW.addCell(new Label(this.UsedJokersCountLabelAddress.Column, this.UsedJokersCountLabelAddress.Row, "Общо използвани жокери"));
            sheetW.addCell(new Label(this.SurrenderCountLabelAddress.Column, this.SurrenderCountLabelAddress.Row, "Oбщо пъти използван \"предавам се\"")); 

            sheetW.addCell(new Number(this.AvgSpentTimeLabelAddress.Column, this.AvgSpentTimeLabelAddress.Row + 1, newAvgSpentTime));
            sheetW.addCell(new Number(this.UsedJokersCountLabelAddress.Column, this.UsedJokersCountLabelAddress.Row + 1, newUsedJokerCount));
            sheetW.addCell(new Number(this.SurrenderCountLabelAddress.Column, this.SurrenderCountLabelAddress.Row + 1, newSurrenderCount));

            workbookW.write();
            workbookW.close();
            workbook.close();
        }
    }

}