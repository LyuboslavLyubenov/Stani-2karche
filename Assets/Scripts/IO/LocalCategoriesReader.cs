namespace IO
{

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Interfaces;

    public class LocalCategoriesReader : IAvailableCategoriesReader
    {
        private readonly string[] RequiredFiles = new [] { "3.xls", "4.xls", "5.xls", "6.xls", "Rating.csv" };

        public void GetAllCategories(Action<string[]> onGetAllCategories)
        {
            if (onGetAllCategories == null)
            {
                throw new ArgumentNullException("onGetAllCategories");
            }

            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\";
            var themesPath = execPath + "LevelData\\теми\\";
            var availableCategories = Directory.GetDirectories(themesPath)
                .Where(this.IsValidLevel)
                .Select(this.GetNameOfCategory)
                .ToArray();
        
            onGetAllCategories(availableCategories);
        }

        private string GetNameOfCategory(string category)
        {
            var categoryNameStartIndex = category.LastIndexOf('\\') + 1;
            var categoryName = category.Substring(categoryNameStartIndex);

            return categoryName;
        }

        private bool IsValidLevel(string path)
        {
            var files = Directory.GetFiles(path).Select(f => f.Substring(path.Length + 1)).ToArray(); 
            var isValidLevel = this.RequiredFiles.All(rf => files.Contains(rf));

            return isValidLevel;
        }
    }

}