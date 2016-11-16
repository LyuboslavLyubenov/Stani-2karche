using System;
using System.Collections.Generic;
using System.IO;
using CSharpJExcel.Jxl;
using System.Linq;
using CSharpJExcel.Jxl.Write;
using CSharpJExcel.Jxl.Biff;

public class BasicExamGeneralStatiticsExporter
{
    const string Directory = "Statistics/";
    const string FileName = "General";
    const string Extension = ".xls";

    readonly string Path = Directory + FileName + Extension;

    readonly JExcelCellPosition AvgSpentTimeOnQuestionAddress = new JExcelCellPosition(0, 1);
    readonly JExcelCellPosition UsedJokersCountAddress = new JExcelCellPosition(1, 1);
    readonly JExcelCellPosition SurrenderCountAddress = new JExcelCellPosition(2, 1);

    BasicExamStatisticsCollector statisticsCollector;

    public BasicExamGeneralStatiticsExporter(BasicExamStatisticsCollector statisticsCollector)
    {
        if (statisticsCollector == null)
        {
            throw new ArgumentNullException("statisticsCollector");
        }
            
        this.statisticsCollector = statisticsCollector;

        if (!File.Exists(Path))
        {
            Workbook.createWorkbook(new FileInfo(Path));
        }
    }

    float ExtractAvgSpentTimeOnQuestion(Sheet sheet)
    {
        var avgSpentTimeCell = sheet.GetCellOrDefault(AvgSpentTimeOnQuestionAddress.Column, AvgSpentTimeOnQuestionAddress.Row); 
        var avgSpentTime = avgSpentTimeCell.getContents();

        if (string.IsNullOrEmpty(avgSpentTime))
        {
            return 0f;
        }

        return float.Parse(avgSpentTime);
    }

    float GetAvgSpentTimeOnQuestion()
    {
        var questionSpentTime = statisticsCollector.QuestionsSpentTime;
        var sumTime = (float)questionSpentTime.Values.Sum(t => t);
        var avgSpentTime = sumTime / (float)questionSpentTime.Count;
        return avgSpentTime;
    }

    float CalculateNewAvgSpentTimeOnQuestion(Sheet sheet)
    {
        var currentGameAvgSpentTimeOnQestion = GetAvgSpentTimeOnQuestion();
        var avgSpentTimeOnQuestion = ExtractAvgSpentTimeOnQuestion(sheet);
        avgSpentTimeOnQuestion += currentGameAvgSpentTimeOnQestion;
        avgSpentTimeOnQuestion /= 2;
        return avgSpentTimeOnQuestion;
    }

    int ExtractUsedJokerCount(Sheet sheet)
    {
        var usedJokerCountCell = sheet.GetCellOrDefault(UsedJokersCountAddress.Row, UsedJokersCountAddress.Column);  
        var usedJokerCount = usedJokerCountCell.getContents();

        if (string.IsNullOrEmpty(usedJokerCount))
        {
            return 0;
        }

        return int.Parse(usedJokerCount);
    }

    int SumUsedJokersCount()
    {
        var jokersUsedTimes = statisticsCollector.JokersUsedTimes;
        return jokersUsedTimes.Sum(jut => jut.Value);
    }

    int CalculateNewUsedJokerCount(Sheet sheet)
    {
        var usedJokersCount = ExtractUsedJokerCount(sheet);
        usedJokersCount += SumUsedJokersCount();
        return usedJokersCount;
    }

    int ExtractSurrenderCount(Sheet sheet)
    {
        var surrenderCountCell = sheet.GetCellOrDefault(SurrenderCountAddress.Row, SurrenderCountAddress.Column);
        var surrenderCount = surrenderCountCell.getContents();

        if (string.IsNullOrEmpty(surrenderCount))
        {
            return 0;
        }

        return int.Parse(surrenderCount);
    }

    public void Export()
    {
        var fileInfo = new FileInfo(Path);
        var fileStream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite);
        var workbook = Workbook.getWorkbook(fileInfo);
        var sheet = workbook.getSheet(0);
        var newAvgSpentTime = CalculateNewAvgSpentTimeOnQuestion(sheet);
        var newUsedJokerCount = CalculateNewUsedJokerCount(sheet);
        var newSurrenderCount = ExtractSurrenderCount(sheet) + 1;

        workbook.close();
        fileStream.Close();

        var exporter = new StatisticsExporter(Path);
        exporter.AddData("Средно време прекарано на въпрос", newAvgSpentTime.ToString());
        exporter.AddData("Общо използвани жокери", newUsedJokerCount.ToString());
        exporter.AddData("Oбщо пъти използван \"предавам се\"", newSurrenderCount.ToString());
        exporter.Export(new SimpleHorizontalSheetFiller(0, 0));
    }
}