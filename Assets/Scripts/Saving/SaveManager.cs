// SaveManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager
{
    public static void SaveAll(string slot = "default")
    {
        var savables = FindAllSavables();
        var saveEntries = new List<SaveEntry>();

        foreach (var savable in savables)
        {
            var key = savable.SaveKey;
            
            if (string.IsNullOrEmpty(key))
                continue;

            var json = savable.CaptureState() ?? string.Empty;

            saveEntries.Add(new SaveEntry { key = key, json = json });
        }

        SaveEntries(slot, saveEntries);
        PlayerPrefs.Save();
    }

    public static void LoadAll(string slot = "default")
    {
        var savedEntries = LoadEntries(slot);
        if (savedEntries == null || savedEntries.Count == 0)
            return;

        var savables = FindAllSavables().ToDictionary(savable => savable.SaveKey, savable => savable);

        foreach (var entry in savedEntries)
        {
            if (savables.TryGetValue(entry.key, out var savable))
            {
                savable.RestoreState(entry.json);
            }
        }
    }

    public static void DeleteSlot(string slot = "default")
    {
        PlayerPrefs.DeleteKey(slot);
        PlayerPrefs.Save();
    }

    public static bool HasSave(string slot = "default")
    {
        return PlayerPrefs.HasKey(slot);
    }

    private static List<ISavable> FindAllSavables()
    {
        return GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ISavable>().ToList();
    }

    private static void SaveEntries(string slot, List<SaveEntry> entries)
    {
        var wrapper = new SaveData { saveEntries = entries };
        var json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(slot, json);
    }

    private static List<SaveEntry> LoadEntries(string slot)
    {
        if (!PlayerPrefs.HasKey(slot))
            return new List<SaveEntry>();

        var json = PlayerPrefs.GetString(slot, string.Empty);

        if (string.IsNullOrEmpty(json))
            return new List<SaveEntry>();

        try
        {
            var wrapper = JsonUtility.FromJson<SaveData>(json);
            return wrapper?.saveEntries ?? new List<SaveEntry>();
        }
        catch
        {
            return new List<SaveEntry>();
        }
    }

    [System.Serializable]
    class SaveData
    {
        public List<SaveEntry> saveEntries = new();
    }

    [System.Serializable]
    class SaveEntry
    {
        public string key;
        public string json;
    }
}