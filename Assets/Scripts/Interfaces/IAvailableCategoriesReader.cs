using System;

namespace Assets.Scripts.Interfaces
{

    public interface IAvailableCategoriesReader
    {
        void GetAllCategories(Action<string[]> onGetAllCategories);
    }

}