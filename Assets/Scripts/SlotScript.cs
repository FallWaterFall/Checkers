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
    public Color WhatIsColor()
    {
        return ocupiedColor;
    }
    public void SetSelection(bool b)
    {
        isSelected = b;
    }
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android && Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.transform.gameObject == this.gameObject)
                    StartCoroutine(BS.MoveSelectedStone((int)this.transform.position.x, (int)this.transform.position.z));
            }
        }
    }
    private void OnMouseDown()
    {
        if (isSelected)
        {
            StartCoroutine(BS.MoveSelectedStone((int)this.transform.position.x, (int)this.transform.position.z));
        }
    }
}
