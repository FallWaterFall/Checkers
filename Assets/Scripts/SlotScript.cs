using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    private BoardScript BS;
    private bool isOcupied = false;
    [SerializeField] private Color ocupiedColor = Color.Empty;
    private bool isSelected = false;
    private void Awake()
    {
        ocupiedColor = Color.Empty;
    }
    private void Start()
    {
        GameObject obj = GameObject.Find("Board");
        BS = obj.GetComponent<BoardScript>();
    }
    public void SetOcupied(bool b, Color color)
    {
        isOcupied = b;
        ocupiedColor = color;
    }
    public bool IsOcupied()
    {
        return isOcupied;
    }
    public bool IsSelected()
    {
        return isSelected;
    }
    public Color WhatIsColor()
    {
        return ocupiedColor;
    }
    public void SetSelection(bool b)
    {
        isSelected = b;
    }
}
