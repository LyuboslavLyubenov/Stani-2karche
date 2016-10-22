using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;

public class LocalCategoriesReader : IAvailableCategoriesReader
{
    readonly string[] RequiredFiles = new string[] { "3.xls", "4.xls", "5.xls", "6.xls", "Rating.csv" };

    public void GetAllCategories(Action<string[]> onGetAllCategories)
    {
        if (onGetAllCategories == null)
        {
            throw new ArgumentNullException("onGetAllCategories");
        }

        var currentDirectory = Directory.GetCurrentDirectory() + "\\LevelData\\теми\\";
        var availableCategories = Directory.GetDirectories(currentDirectory)
            .Where(IsValidLevel)
            .Select((categoryFilePath) => GetNameOfCategory(categoryFilePath))
            .ToArray();
        
        onGetAllCategories(availableCategories);
    }

    string GetNameOfCategory(string category)
    {
        var categoryNameStartIndex = category.LastIndexOf('\\') + 1;
        var categoryName = category.Substring(categoryNameStartIndex);

        return categoryName;
    }


    bool IsValidLevel(string path)
    {
        var files = Directory.GetFiles(path).Select(f => f.Substring(path.Length + 1)).ToArray(); 
        var isValidLevel = files.All(f => RequiredFiles.Contains(f));
        return isValidLevel;
    }
}