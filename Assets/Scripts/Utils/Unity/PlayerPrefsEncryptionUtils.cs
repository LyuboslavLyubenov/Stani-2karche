namespace Utils.Unity
{

    using System;
    using System.Security.Cryptography;

    using SecuritySettings;

    using UnityEngine;

    public class PlayerPrefsEncryptionUtils
    {
        private readonly Type[] supportedTypes = new [] { typeof(int), typeof(string), typeof(float) };

        private PlayerPrefsEncryptionUtils()
        {
        
        }
        
        public static void DeleteKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
            PlayerPrefs.DeleteKey(encryptedKey);
        }

        public static bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
            return PlayerPrefs.HasKey(encryptedKey);
        }

        public static void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
            var encryptedValue = CipherUtility.Encrypt<RijndaelManaged>(value, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
            PlayerPrefs.SetString(encryptedKey, encryptedValue);
        }

        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
            var encryptedValue = PlayerPrefs.GetString(encryptedKey);
            var decryptedValue = CipherUtility.Decrypt<RijndaelManaged>(encryptedValue, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);

            return decryptedValue;
        }
    }

}
