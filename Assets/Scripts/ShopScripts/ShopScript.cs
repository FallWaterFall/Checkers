using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    [SerializeField] List<Sprite> SkinsList = new List<Sprite>();
    [SerializeField] GameObject ShopItemPref;
    private SaveData data;
    private List<int> UnlockedSkinsList = new List<int>();
    private List<GameObject> ShopItems = new List<GameObject>();
    private int CurSelectedSkin = 0;
    public void Start()
    {
        //Loading data from file
        data = SaveSystem.Load();
        if (data.moneyAmount < 0)
        {
            data = new SaveData(10, new List<int>(), 0);
            SaveSystem.Save(data);
        }
        this.transform.GetChild(0).GetComponent<MoneyShowScript>().UpdateMoneyText(data.moneyAmount);

        Debug.Log(data.moneyAmount);
        Debug.Log("SkinsNum list before clearing :");
        for (int i = 0; i < data.UnlockedSkins.Count; i++)
            Debug.Log(data.UnlockedSkins[i]);

        //data.UnlockedSkins.Clear();
        //data.UnlockedSkins.Add(0);

        //Instantiate Items in Shop
        for (int i = 0; i < SkinsList.Count; i++)
        {
            var obj = Instantiate(ShopItemPref, this.transform.GetChild(2).transform.GetChild(0));
            obj.GetComponent<ShopItemScript>().SetSkinNum(i);
            if (i == 0) obj.GetComponent<ShopItemScript>().SetButton(2);
            else if (data.UnlockedSkins.Contains(i)) obj.GetComponent<ShopItemScript>().SetButton(1);
            else obj.GetComponent<ShopItemScript>().SetButton(0);
            
            obj.transform.GetChild(0).GetComponent<Image>().sprite = SkinsList[i];

            ShopItems.Add(obj);
        }
    }
    public void AddToList(int i)
    {
        data.UnlockedSkins.Add(i);
        for (int j = 0; j < data.UnlockedSkins.Count; j++)
            Debug.Log(data.UnlockedSkins[j]);
    }
    public void UpdateSelectedSkin(int num)
    {
        ShopItems[CurSelectedSkin].GetComponent<ShopItemScript>().SetButton(1);
        CurSelectedSkin = num;
        data.selectedSkin = CurSelectedSkin;
    }
    public void BackToMenu()
    {
        Debug.Log(data.selectedSkin);
        SaveSystem.Save(data);
        SceneManager.LoadScene(0);
    }
    public void OnApplicationQuit()
    {
        SaveSystem.Save(data);
    }
    public int GetMoneyAmount()
    {
        return data.moneyAmount;
    }
    public void ChengeMoneyAmount(int amount)
    {
        data.moneyAmount += amount;
    }
}
