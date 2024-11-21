using UnityEngine;
using Newtonsoft.Json;
using System;

public static class PlayerPrefsData
{
    private static AES128 aes128 = new AES128();

    public static void Set<T>(string key, T value, bool encryption = true) where T : IComparable
    {
        string jsonValue = JsonConvert.SerializeObject(value);
 
        string savingKey = encryption ? aes128.Encrypt(key) : key;
        string savingValue = encryption ? aes128.Encrypt(jsonValue) : jsonValue;
 
        PlayerPrefs.SetString(savingKey, savingValue);
    }
 
    public static T Get<T>(string originalKey, T defaultValue = default(T), bool encryption = true) where T : IComparable
    {
        string savedKey = encryption ? aes128.Encrypt(originalKey) : originalKey;
        string savedValue;
 
        if(!PlayerPrefs.HasKey(savedKey))
        {
            return defaultValue;
        }
 
        savedValue = PlayerPrefs.GetString(savedKey, "");
        string originalValue = encryption ? aes128.Decrypt(savedValue) : savedValue;
 
        if(originalValue == "") return defaultValue;
 
        return JsonConvert.DeserializeObject<T>(originalValue);
    }
 
    public static bool HasKey(string key, bool encryption = true)
    {
        key = encryption ? aes128.Encrypt(key) : key;
        return PlayerPrefs.HasKey(key);
    }
    public static void DeleteKey(string key, bool encryption = true)
    {
        key = encryption ? aes128.Encrypt(key) : key;
        PlayerPrefs.DeleteKey(key);
    }
}
