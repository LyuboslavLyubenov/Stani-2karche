using System;
using UnityEngine;
using System.Security.Cryptography;

public class PlayerPrefsEncryptionUtils
{
    readonly Type[] SupportedTypes = new [] { typeof(int), typeof(string), typeof(float) };

    PlayerPrefsEncryptionUtils()
    {
        
    }

    static void ValidateNotEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(paramName + " cannot be empty");
        }
    }

    public static void DeleteKey(string key)
    {
        ValidateNotEmpty(key, "Key");
        var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
        PlayerPrefs.DeleteKey(encryptedKey);
    }

    public static bool HasKey(string key)
    {
        ValidateNotEmpty(key, "Key");

        var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
        return PlayerPrefs.HasKey(encryptedKey);
    }

    public static void SetString(string key, string value)
    {
        ValidateNotEmpty(key, "Key");
        ValidateNotEmpty(value, "Value");

        var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
        var encryptedValue = CipherUtility.Encrypt<RijndaelManaged>(value, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
        PlayerPrefs.SetString(encryptedKey, encryptedValue);
    }

    public static string GetString(string key)
    {        
        ValidateNotEmpty(key, "Key");

        var encryptedKey = CipherUtility.Encrypt<RijndaelManaged>(key, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);
        var encryptedValue = PlayerPrefs.GetString(encryptedKey);
        var decryptedValue = CipherUtility.Decrypt<RijndaelManaged>(encryptedValue, SecuritySettings.PLAYERPREFS_PASSWORD, SecuritySettings.SALT);

        return decryptedValue;
    }
}
