using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemScript : MonoBehaviour
{
    private ShopScript SS;
    private int SkinNum = -1;
    private void Start()
    {
        SS = this.transform.parent.transform.parent.transform.parent.GetComponent<ShopScript>();
    }
    public void SetSkinNum(int i)
    {
        SkinNum = i;
    }
    public void SetButton(int ButtonNum)
    {
        if (ButtonNum == 0) //Skin NOT bought
        {
            this.transform.GetChild(1).gameObject.SetActive(true);
            this.transform.GetChild(2).gameObject.SetActive(false);
            return;
        }
        if (ButtonNum == 1) //Skin NOT selected
        {
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(2).gameObject.SetActive(true);
            return;
        }
        if (ButtonNum == 2) //Skin Selected
        {
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(2).gameObject.SetActive(false);
            return;
        }
    }
    public void OnBuy()
    {
        if (SS.GetMoneyAmount() >= 100)
        {
            SS.ChengeMoneyAmount(-100);
            SS.AddToList(SkinNum);
            this.transform.GetChild(1).gameObject.SetActive(false);
            this.transform.GetChild(2).gameObject.SetActive(true);
        } else {Debug.Log("NOT ENOUGHT RUBY");}
    }
    public void UseSkin()
    {
        this.transform.GetChild(2).gameObject.SetActive(false);
        SS.UpdateSelectedSkin(SkinNum);
        DataBetweenScenes.Skin = SkinNum;
    }
}
