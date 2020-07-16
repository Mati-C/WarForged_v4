using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public PlayerData data;

    public string file = "player.text";
    public bool resetOnPlay;

    private void Awake()
    {
        if (resetOnPlay) ResetData();
    }

    public void ResetData()
    {
        Load();
        data.swordLevel = 0;
        data.currentExp = 0;
        data.expEarned = 0;
        Save();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data);
        WriteToFile(file, json);
    }

    public void Load()
    {
        data = new PlayerData();
        string json = ReadFromFile(file);
        JsonUtility.FromJsonOverwrite(json, data);
    }

    public void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    public string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else Debug.LogWarning("FILE NOT FOUND");

        return "";
    }

    public string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
}
