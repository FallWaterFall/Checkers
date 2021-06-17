using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    [SerializeField] private List<Sprite> ModelsList = new List<Sprite>();
    [SerializeField] private List<GameObject> ModelsObjList = new List<GameObject>();
    [SerializeField] private List<Color32> ColorsList = new List<Color32>();
    [SerializeField] private List<Sprite> ImagesList = new List<Sprite>();
    [SerializeField] private GameObject ShopModelItemPref;
    private GameObject RepresentationObj;
    private SaveData data;
    private Transform ModelsContent, ColorsContent, ImagesContent;
    private List<int> UnlockedModelsList = new List<int>();
    private List<GameObject> ShopModelItems = new List<GameObject>();
    private List<GameObject> ShopColorItems = new List<GameObject>();
    private List<GameObject> ShopImageItems = new List<GameObject>();
    private int CurSelectedModel = 0, CurSelectedColor = 0, CurSelectedImage = 0;
    public void Start()
    {
        ModelsContent = this.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0);
        ColorsContent = this.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0);
        ImagesContent = this.transform.GetChild(2).transform.GetChild(2).transform.GetChild(0);
        //Loading data from file
        data = SaveSystem.Load();
        if (data.moneyAmount > 0)
        {
            data = new SaveData(1000, new List<int>(), new List<int>(), new List<int>(), 0, 0, 0);
            data.UnlockedModels.Add(0);
            data.UnlockedColors.Add(0);
            data.UnlockedImages.Add(0);
            SaveSystem.Save(data);
        }
        this.transform.GetChild(0).GetComponent<MoneyShowScript>().UpdateMoneyText(data.moneyAmount);

        //Instantiate Items in Models Shop
        for (int i = 0; i < ModelsList.Count; i++)
        {
            var obj = Instantiate(ShopModelItemPref, ModelsContent);
            obj.GetComponent<ShopItemScript>().SetSkinTypeAndNum(0, i);
            if (i == 0) obj.GetComponent<ShopItemScript>().SetButton(2);
            else if (data.UnlockedModels.Contains(i)) obj.GetComponent<ShopItemScript>().SetButton(1);
            else obj.GetComponent<ShopItemScript>().SetButton(0);
            
            obj.transform.GetChild(0).GetComponent<Image>().sprite = ModelsList[i];

            ShopModelItems.Add(obj);
        }
        //Instantiate Items in Colors Shop
        for (int i = 0; i < ColorsList.Count; i++)
        {
            var obj = Instantiate(ShopModelItemPref, ColorsContent);
            obj.GetComponent<ShopItemScript>().SetSkinTypeAndNum(1, i);
            if (i == 0) obj.GetComponent<ShopItemScript>().SetButton(2);
            else if (data.UnlockedColors.Contains(i)) obj.GetComponent<ShopItemScript>().SetButton(1);
            else obj.GetComponent<ShopItemScript>().SetButton(0);
            
            obj.transform.GetChild(0).GetComponent<Image>().color = ColorsList[i];

            ShopColorItems.Add(obj);
        }
        //Instantiate Items in Images Shop
        for (int i = 0; i < ImagesList.Count; i++)
        {
            var obj = Instantiate(ShopModelItemPref, ImagesContent);
            obj.GetComponent<ShopItemScript>().SetSkinTypeAndNum(2, i);
            if (i == 0) obj.GetComponent<ShopItemScript>().SetButton(2);
            else if (data.UnlockedImages.Contains(i)) obj.GetComponent<ShopItemScript>().SetButton(1);
            else obj.GetComponent<ShopItemScript>().SetButton(0);
            
            obj.transform.GetChild(0).GetComponent<Image>().sprite = ImagesList[i];

            ShopImageItems.Add(obj);
        }
        //Show model tab as standart
        ShowModelsShopButton();
        //Create representation for
        RepresentationObj = this.transform.GetChild(4).gameObject;
        ReCreateRepObj();
    }
    public void AddToList(int type, int i)
    {
        if (type == 1)
        {
            data.UnlockedModels.Add(i);
            for (int j = 0; j < data.UnlockedModels.Count; j++)
                Debug.Log(data.UnlockedModels[j]);
            return;
        }
        if (type == 2)
        {
            data.UnlockedColors.Add(i);
            for (int j = 0; j < data.UnlockedColors.Count; j++)
                Debug.Log(data.UnlockedColors[j]);
            return;
        }
        if (type == 3)
        {
            data.UnlockedImages.Add(i);
            for (int j = 0; j < data.UnlockedImages.Count; j++)
                Debug.Log(data.UnlockedImages[j]);
        }
    }
    public void UpdateSelectedItem(int type, int num)
    {
        if (type == 0)
        {
            ShopModelItems[CurSelectedModel].GetComponent<ShopItemScript>().SetButton(1);
            CurSelectedModel = num;
            data.selectedModel = CurSelectedModel;
        }
        if (type == 1)
        {
            ShopColorItems[CurSelectedColor].GetComponent<ShopItemScript>().SetButton(1);
            CurSelectedColor = num;
            data.selectedColor = CurSelectedColor;
        }
        if (type == 2)
        {
            ShopImageItems[CurSelectedImage].GetComponent<ShopItemScript>().SetButton(1);
            CurSelectedImage = num;
            data.selectedImage = CurSelectedImage;
        }
        UpdateRepresentation(type);
    }
    private void ReCreateRepObj()
    {
        var repObj = Instantiate(ModelsObjList[data.selectedModel]);
        repObj.transform.SetParent(RepresentationObj.transform);
        repObj.transform.localPosition = new Vector3(0, 0, -65); 
        if (data.selectedModel == 0)
        {
            repObj.transform.eulerAngles = new Vector3(-35, 0, 0);
            repObj.transform.localScale = new Vector3(100, 20, 100);
        }
        if (data.selectedModel == 1)
        {
            repObj.transform.eulerAngles = new Vector3(-125, 0, 0);
            repObj.transform.localScale = new Vector3(10000, 10000, 2000);
        }
        repObj.GetComponent<Renderer>().material.color = ColorsList[data.selectedColor];
    }
    private void UpdateRepresentation(int type)
    {
        var repObj = RepresentationObj.transform.GetChild(0).gameObject;

        if (type == 0)
        {
            Destroy(repObj);
            ReCreateRepObj();
        }
        if (type == 1)
        {
            repObj.GetComponent<Renderer>().material.color = ColorsList[CurSelectedColor];
        }
        if (type == 2)
        {
            Debug.Log("Chenge Image");
        }
    }
    public void BackToMenu()
    {
        Debug.Log(data.selectedModel);
        SaveSystem.Save(data);
        SceneManager.LoadScene(0);
    }
    public void ShowModelsShopButton()
    {
        ModelsContent.transform.parent.gameObject.SetActive(true);
        ColorsContent.transform.parent.gameObject.SetActive(false);
        ImagesContent.transform.parent.gameObject.SetActive(false);
    }
    public void ShowColorsShopButton()
    {
        ModelsContent.transform.parent.gameObject.SetActive(false);
        ColorsContent.transform.parent.gameObject.SetActive(true);
        ImagesContent.transform.parent.gameObject.SetActive(false);
    }
    public void ShowImagesShopButton()
    {
        ModelsContent.transform.parent.gameObject.SetActive(false);
        ColorsContent.transform.parent.gameObject.SetActive(false);
        ImagesContent.transform.parent.gameObject.SetActive(true);
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
        this.transform.GetChild(0).GetComponent<MoneyShowScript>().UpdateMoneyText(data.moneyAmount);
    }
}
