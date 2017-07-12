namespace Network
{

    using System;
    using System.Collections;

    using CielaSpike.Thread_Ninja;

    using DTOs.KinveyDtoObjs;

    using Extensions;

    using Interfaces.Network.Kinvey;

    using UnityEngine;

    using Utils;

    public class KinveyWrapper : IKinveyWrapper
    {
        public bool IsLoggedIn
        {
            get
            {
                return string.IsNullOrEmpty(RequesterUtils.SessionAuth);
            }
        }

        private IEnumerator LoginCoroutine(string username, string password, Action<_UserReceivedData> onLoggedIn, Action<Exception> onError)
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

        private IEnumerator LogoutCoroutine(Action onLogout, Action<Exception> onError)
        {
            const string url = RequesterUtils.KinveyUrl + "user/" + RequesterUtils.AppKey + "/_logout";

            var requester = RequesterUtils.ConfigRequester(url, "POST", null, true);
            Exception exception = null;

            try
            {
                requester.GetRequestResult();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                RequesterUtils.RemoveSessionAuth();
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

        private IEnumerator RegisterCoroutine(string username, string password, Action<_UserReceivedData> onRegister, Action<Exception> onError)
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

        private IEnumerator CreateEntityCoroutine(string tableName, string json, Action onCreated, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "appdata/" + RequesterUtils.AppKey + "/" + tableName;

            var requester = RequesterUtils.ConfigRequester(url, "POST", json, true);
            Exception exception = null;

            try
            {
                requester.GetRequestResult();
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

        private IEnumerator RetrieveEntityCoroutine<T>(string tableName, string id, Action<_KinveyEntity<T>[]> onRetrieved, Action<Exception> onError)
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
            var objs = JsonArrayUtility.ArrayFromJson<T>(json);
            var kinveyDetails = JsonArrayUtility.ArrayFromJson<_KinveyEntityDetails>(json);
            var entities = new _KinveyEntity<T>[objs.Length];

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                var detail = kinveyDetails[i];
                entities[i] = new _KinveyEntity<T>() { Entity = obj, EntityDetails = detail };
            }

            onRetrieved(entities);
        }

        private _KinveyEntity<T> Parse<T>(string json)
        {
            var entity = JsonUtility.FromJson<T>(json);
            var details = JsonUtility.FromJson<_KinveyEntityDetails>(json);
            return new _KinveyEntity<T>() { Entity = entity, EntityDetails = details };
        }

        private IEnumerator UpdateEntityCoroutine<T>(string tableName, string id, T entity, Action onSuccessfullyUpdated, Action<Exception> onError)
        {
            var url = RequesterUtils.KinveyUrl + "appdata/" + RequesterUtils.AppKey + "/" + tableName + "/" + id;
            var json = JsonUtility.ToJson(entity);
            var requester = RequesterUtils.ConfigRequester(url, "PUT", json, true);
            Exception exception = null;

            try
            {
                requester.GetRequestResult();
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

        private IEnumerator DeleteEntityCoroutine(string tableName, string id, Action<_DeletedCount> onSuccessfullyDeleted, Action<Exception> onError)
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

        private IEnumerator DoesUsernameExistsCoroutine(string username, Action<_UsersnameExistence> onSuccessfullyChecked, Action<Exception> onError)
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
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("username");
            }

            if (onRegistered == null)
            {
                throw new ArgumentNullException("onRegistered");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }
            
            ThreadUtils.Instance.RunOnBackgroundThread(this.RegisterCoroutine(username, password, onRegistered, onError));
        }

        public void LoginAsync(string username, string password, Action<_UserReceivedData> onLoggedIn, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("username");
            }

            if (onLoggedIn == null)
            {
                throw new ArgumentNullException("onLoggedIn");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.LoginCoroutine(username, password, onLoggedIn, onError));
        }

        public void Logout(Action onLogout, Action<Exception> onError)
        {
            if (onLogout == null)
            {
                throw new ArgumentNullException("onLoggedIn");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.LogoutCoroutine(onLogout, onError));
        }

        public void CreateEntityAsync(string tableName, string json, Action onCreated, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException("json");
            }

            if (onCreated == null)
            {
                throw new ArgumentNullException("onCreated");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.CreateEntityCoroutine(tableName, json, onCreated, onError));
        }

        public void CreateEntityAsync<T>(string tableName, T entity, Action onCreated, Action<Exception> onError)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("tableName");
            }
           
            var json = JsonUtility.ToJson(entity);
            this.CreateEntityAsync(tableName, json, onCreated, onError);
        }

        public void RetrieveEntityAsync<T>(string tableName, string id, Action<_KinveyEntity<T>[]> onRetrieved, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (onRetrieved == null)
            {
                throw new ArgumentNullException("onRetrieved");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.RetrieveEntityCoroutine<T>(tableName, id, onRetrieved, onError));
        }

        public void UpdateEntityAsync<T>(string tableName, string id, T entity, Action onSuccessfullyUpdated, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("json");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("onCreated");
            }

            if (onSuccessfullyUpdated == null)
            {
                throw new ArgumentNullException("onCreated");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.UpdateEntityCoroutine<T>(tableName, id, entity, onSuccessfullyUpdated, onError));
        }

        public void DeleteEntityAsync(string tableName, string id, Action<_DeletedCount> onSuccessfullyDeleted, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            if (onSuccessfullyDeleted == null)
            {
                throw new ArgumentNullException("onSuccessfullyDeleted");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.DeleteEntityCoroutine(tableName, id, onSuccessfullyDeleted, onError));
        }

        public void DoesUsernameExistsAsync(string username, Action<_UsersnameExistence> onSuccessfullyChecked, Action<Exception> onError)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("tableName");
            }

            if (onSuccessfullyChecked == null)
            {
                throw new ArgumentNullException("onCreated");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            ThreadUtils.Instance.RunOnBackgroundThread(this.DoesUsernameExistsCoroutine(username, onSuccessfullyChecked, onError));
        }
    }
}