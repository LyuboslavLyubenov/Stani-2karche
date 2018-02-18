namespace Utils.Unity
{

    using System;
    using System.Collections;
    using System.Security.Cryptography;

    using CielaSpike.Thread_Ninja;

    using SecuritySettings;

    public class EncryptionUtils 
    {
        EncryptionUtils()
        {   
        }

        public static void DecryptMessageAsync(string message, string key, Action<string> onDecrypted)
        {
            if (onDecrypted == null)
            {
                throw new ArgumentNullException("onDecrypted");
            }

            if (string.IsNullOrEmpty(message))
            {
                onDecrypted(message);
                return;
            }

            NetworkTransportUtilsDummyClass.Instance.StartCoroutineAsync(DecryptMessageAsyncCoroutine(message, key, onDecrypted));
        }

        private static IEnumerator DecryptMessageAsyncCoroutine(string message, string key, Action<string> onDecrypted)
        {
            var decryptedMessage = string.Empty;

            if (!string.IsNullOrEmpty(message))
            {
                decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(message, key, SecuritySettings.SALT);
            }

            yield return Ninja.JumpToUnity;
            onDecrypted(decryptedMessage);
        }

        public static void EncryptMessageAsync(string message, string key, Action<string> onEncrypted)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be empty");
            }

            if (onEncrypted == null)
            {
                throw new ArgumentNullException("onEncrypted");
            }

            NetworkTransportUtilsDummyClass.Instance.StartCoroutineAsync(EncryptMessageAsyncCoroutine(message, key, onEncrypted));
        }

        private static IEnumerator EncryptMessageAsyncCoroutine(string message, string key, Action<string> onEncrypted)
        {
            var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, key, SecuritySettings.SALT);
            yield return Ninja.JumpToUnity;
            onEncrypted(encryptedMessage);
        }


    }
}