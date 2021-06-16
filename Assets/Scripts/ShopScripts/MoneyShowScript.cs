using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyShowScript : MonoBehaviour
{
    public void UpdateMoneyText(int amount)
    {
        this.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = amount.ToString();
    }
}
