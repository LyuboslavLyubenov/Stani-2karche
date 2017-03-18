namespace Interfaces
{

    using System;

    public interface IAvailableCategoriesReader
    {
        void GetAllCategories(Action<string[]> onGetAllCategories);
    }

}