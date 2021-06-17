using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class SaveSystem
{
    public static void Save(SaveData data)
    {
        IFormatter formatter = new BinaryFormatter();
        Debug.Log(Application.persistentDataPath + "/player.data");
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, "/player.data"));
        formatter.Serialize(file, data);
        file.Close();
    } 
    public static SaveData Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, "/player.data")))
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream file = File.OpenRead(string.Concat(Application.persistentDataPath, "/player.data"));
            SaveData newSaveData = (SaveData)formatter.Deserialize(file);
            file.Close();
            return newSaveData;
        }
        else return new SaveData(10, new List<int>(), new List<int>(), new List<int>(), 0, 0, 0);
    } 
}
[System.Serializable]
public class SaveData
{
    public int moneyAmount;
    public List<int> UnlockedModels;
    public List<int> UnlockedColors;
    public List<int> UnlockedImages;
    public int selectedModel;
    public int selectedColor;
    public int selectedImage;
    public SaveData(int _moneyAmount, List<int> _UnlockedModels, List<int> _UnlockedColors, List<int> _UnlockedImages, int _selectedModel, int _selectedColor, int _selectedImage)
    {
        moneyAmount = _moneyAmount;
        UnlockedModels = _UnlockedModels;
        UnlockedColors = _UnlockedColors;
        UnlockedImages = _UnlockedImages;
        selectedModel = _selectedModel;
        selectedColor = _selectedColor;
        selectedImage = _selectedImage;
    }
}