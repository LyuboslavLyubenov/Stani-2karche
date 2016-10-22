using System;

public interface IAvailableCategoriesReader
{
    void GetAllCategoriesAsync(Action<string[]> onGetAllCategories);
}