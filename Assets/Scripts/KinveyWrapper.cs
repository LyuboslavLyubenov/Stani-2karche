using System;
using System.Collections;

using UnityEngine;

namespace Assets.Scripts
{

    using Assets.CielaSpike.Thread_Ninja;
    using Assets.Scripts.KinveySerializableObj;
    using Assets.Scripts.Utils;

    public class KinveyWrapper : MonoBehaviour
    {
        static KinveyWrapper instance;

        public static KinveyWrapper Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject();
                    obj.name = "KinveyConnectorWrapper";
                    instance = obj.AddComponent<KinveyWrapper>();
                }

                return instance;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return string.IsNullOrEmpty(RequesterUtils.SessionAuth);
            }
        }

        IEnumerator LoginCoroutine(string username, string password, Action<_UserReceivedData> onLoggedIn, Action<Exception> onError)
        {
            const string url = RequesterUtils.KinveyUrl + "user/" + RequesterUtils.AppKey + "/login";

            //set user credentials
            var userCredentials = new _UserCredentials() { username = username, password = password };
            var data = JsonUtility.ToJson(userCredentials);
            var requester = RequesterUtils.ConfigRequester(url, "POST", data, false);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();    
            }
            catch (Exception ex)
            {
                exception = ex;    
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;

            var userReceivedData = JsonUtility.FromJson<_UserReceivedData>(requestResult);
            onLoggedIn(userReceivedData);
            RequesterUtils.SessionAuth = userReceivedData._kmd.authtoken;
        }

        IEnumerator LogoutCoroutine(Action onLogout, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "user/" + RequesterUtils.AppKey + "/_logout";
            var requester = RequesterUtils.ConfigRequester(url, "POST", null, true);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;

            RequesterUtils.RemoveSessionAuth();
            onLogout();
        }

        IEnumerator RegisterCoroutine(string username, string password, Action<_UserReceivedData> onRegister, Action<Exception> onError)
        {
            const string url = RequesterUtils.KinveyUrl + "user/" + RequesterUtils.AppKey;

            var credentials = new _UserCredentials() { username = username, password = password };
            var credentialsJSON = JsonUtility.ToJson(credentials);
            var requester = RequesterUtils.ConfigRequester(url, "POST", credentialsJSON, false);

            Exception exception = null;
            string requestResult = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;

            var receivedData = JsonUtility.FromJson<_UserReceivedData>(requestResult);
            onRegister(receivedData);
        }

        IEnumerator CreateEntityCoroutine(string tableName, string json, Action onCreated, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "appdata/" + RequesterUtils.AppKey + "/" + tableName;
            var requester = RequesterUtils.ConfigRequester(url, "POST", json, true);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;
            onCreated();
        }

        IEnumerator RetrieveEntityCoroutine<T>(string tableName, string id, Action<_KinveyEntity<T>[]> onRetrieved, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "appdata/" + RequesterUtils.AppKey + "/" + tableName + "/";

            if (!string.IsNullOrEmpty(id))
            {
                url += id;
            }

            var requester = RequesterUtils.ConfigRequester(url, "GET", null, true);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;

            if (!string.IsNullOrEmpty(id))
            {
                var entity = this.Parse<T>(requestResult);
                onRetrieved(new _KinveyEntity<T>[] { entity });
                yield break;
            }

            var json = "{\"Items\":" + requestResult + "}";
            var objs = JsonHelper.FromJson<T>(json);
            var kinveyDetails = JsonHelper.FromJson<_KinveyEntityDetails>(json);
            var entities = new _KinveyEntity<T>[objs.Length];

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                var detail = kinveyDetails[i];
                entities[i] = new _KinveyEntity<T>() { Entity = obj, EntityDetails = detail };
            }

            onRetrieved(entities);
        }

        _KinveyEntity<T> Parse<T>(string json)
        {
            var entity = JsonUtility.FromJson<T>(json);
            var details = JsonUtility.FromJson<_KinveyEntityDetails>(json);
            return new _KinveyEntity<T>() { Entity = entity, EntityDetails = details };
        }

        IEnumerator UpdateEntityCoroutine<T>(string tableName, string id, T entity, Action onSuccessfullyUpdated, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "appdata/" + RequesterUtils.AppKey + "/" + tableName + "/" + id;
            var json = JsonUtility.ToJson(entity);
            var requester = RequesterUtils.ConfigRequester(url, "PUT", json, true);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;   
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;
            onSuccessfullyUpdated();
        }

        IEnumerator DeleteEntityCoroutine(string tableName, string id, Action<_DeletedCount> onSuccessfullyDeleted, Action<Exception> onError)
        {
            var url = string.Join("", new string[] { RequesterUtils.KinveyUrl, "appdata/", RequesterUtils.AppKey, "/", tableName, "/", id });
            var requester = RequesterUtils.ConfigRequester(url, "DELETE", null, true);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;

            var deletedCount = JsonUtility.FromJson<_DeletedCount>(requestResult);
            onSuccessfullyDeleted(deletedCount);
        }

        IEnumerator DoesUsernameExistsCoroutine(string username, Action<_UsersnameExistence> onSuccessfullyChecked, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "rpc/" + RequesterUtils.AppKey + "/check-username-exists";
            var json = "{ \"username\": \"" + username + "\" }";
            var requester = RequesterUtils.ConfigRequester(url, "POST", json, false);
            string requestResult = null;
            Exception exception = null;

            try
            {
                requestResult = requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return Ninja.JumpToUnity;
                onError(exception);
                yield break;
            }

            yield return Ninja.JumpToUnity;
            var usernameExistence = JsonUtility.FromJson<_UsersnameExistence>(requestResult);
            onSuccessfullyChecked(usernameExistence);
        }

        public void RegisterAsync(string username, string password, Action<_UserReceivedData> onRegistered, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(username, "username");
            ValidationUtils.ValidateStringNotNullOrEmpty(password, "password");
            ValidationUtils.ValidateObjectNotNull(onRegistered, "onRegistered");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.RegisterCoroutine(username, password, onRegistered, onError));
        }

        public void LoginAsync(string username, string password, Action<_UserReceivedData> onLoggedIn, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(username, "username");
            ValidationUtils.ValidateStringNotNullOrEmpty(password, "password");
            ValidationUtils.ValidateObjectNotNull(onLoggedIn, "onLoggedIn");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.LoginCoroutine(username, password, onLoggedIn, onError));
        }

        public void Logout(Action onLogout, Action<Exception> onError)
        {
            ValidationUtils.ValidateObjectNotNull(onLogout, "onLogout");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.LogoutCoroutine(onLogout, onError));
        }

        public void CreateEntityAsync(string tableName, string json, Action onCreated, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(tableName, "tableName");
            ValidationUtils.ValidateStringNotNullOrEmpty(json, "json");
            ValidationUtils.ValidateObjectNotNull(onCreated, "onCreated");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.CreateEntityCoroutine(tableName, json, onCreated, onError));
        }

        public void CreateEntityAsync<T>(string tableName, T entity, Action onCreated, Action<Exception> onError)
        {
            ValidationUtils.ValidateObjectNotNull(entity, "entity");

            var json = JsonUtility.ToJson(entity);
            this.CreateEntityAsync(tableName, json, onCreated, onError);
        }

        public void RetrieveEntityAsync<T>(string tableName, string id, Action<_KinveyEntity<T>[]> onRetrieved, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(tableName, "tableName");
            ValidationUtils.ValidateObjectNotNull(onRetrieved, "onRetrieved");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");
        
            this.StartCoroutineAsync(this.RetrieveEntityCoroutine<T>(tableName, id, onRetrieved, onError));
        }

        public void UpdateEntityAsync<T>(string tableName, string id, T entity, Action onSuccessfullyUpdated, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(tableName, "tableName");
            ValidationUtils.ValidateStringNotNullOrEmpty(id, "id");
            ValidationUtils.ValidateObjectNotNull(entity, "entity");
            ValidationUtils.ValidateObjectNotNull(onSuccessfullyUpdated, "onSuccessfullyUpdated");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.UpdateEntityCoroutine<T>(tableName, id, entity, onSuccessfullyUpdated, onError));
        }

        public void DeleteEntityAsync(string tableName, string id, Action<_DeletedCount> onSuccessfullyDeleted, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(tableName, "tableName");
            ValidationUtils.ValidateObjectNotNull(onSuccessfullyDeleted, "onSuccessfullyDeleted");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.DeleteEntityCoroutine(tableName, id, onSuccessfullyDeleted, onError));
        }

        public void DoesUsernameExistsAsync(string username, Action<_UsersnameExistence> onSuccessfullyChecked, Action<Exception> onError)
        {
            ValidationUtils.ValidateStringNotNullOrEmpty(username, "username");
            ValidationUtils.ValidateObjectNotNull(onSuccessfullyChecked, "onSuccessfullyChecked");
            ValidationUtils.ValidateObjectNotNull(onError, "onError");

            this.StartCoroutineAsync(this.DoesUsernameExistsCoroutine(username, onSuccessfullyChecked, onError));
        }
    }

}

