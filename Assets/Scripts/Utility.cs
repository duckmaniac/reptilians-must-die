using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public interface IDeepCloneable<T>
    {
        T DeepCopy();
    }

    public static List<T> DeepCopyList<T>(List<T> originalList) where T : IDeepCloneable<T>
    {
        List<T> copiedList = new List<T>();
        foreach (T originalItem in originalList)
        {
            copiedList.Add(originalItem.DeepCopy());
        }
        return copiedList;
    }

    public static T LoadFromMemory<T>(string fileName)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
        return default;
    }

    public static void SaveToMemory<T>(string fileName, T value)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonConvert.SerializeObject(value);
        System.IO.File.WriteAllText(path, json);
    }

    public static T LoadJsonFromResources<T>(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            return JsonConvert.DeserializeObject<T>(jsonFile.text);
        }
        return default;
    }

    public static void Swap<T>(List<T> list, int index1, int index2)
    {
        T temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
}
