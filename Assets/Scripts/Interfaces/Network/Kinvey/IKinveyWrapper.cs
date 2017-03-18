namespace Interfaces.Network.Kinvey
{

    using System;

    using DTOs.KinveyDtoObjs;

    public interface IKinveyWrapper
    {
        bool IsLoggedIn { get; }

        void RegisterAsync(
            string username,
            string password,
            Action<_UserReceivedData> onRegistered,
            Action<Exception> onError);

        void LoginAsync(
            string username,
            string password,
            Action<_UserReceivedData> onLoggedIn,
            Action<Exception> onError);

        void Logout(Action onLogout, Action<Exception> onError);

        void CreateEntityAsync(string tableName, string json, Action onCreated, Action<Exception> onError);

        void CreateEntityAsync<T>(string tableName, T entity, Action onCreated, Action<Exception> onError);

        void RetrieveEntityAsync<T>(
            string tableName,
            string id,
            Action<_KinveyEntity<T>[]> onRetrieved,
            Action<Exception> onError);

        void UpdateEntityAsync<T>(
            string tableName,
            string id,
            T entity,
            Action onSuccessfullyUpdated,
            Action<Exception> onError);

        void DeleteEntityAsync(
            string tableName,
            string id,
            Action<_DeletedCount> onSuccessfullyDeleted,
            Action<Exception> onError);

        void DoesUsernameExistsAsync(
            string username,
            Action<_UsersnameExistence> onSuccessfullyChecked,
            Action<Exception> onError);
    }

}