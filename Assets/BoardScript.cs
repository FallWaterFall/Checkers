using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color
{
        White,
        Black,
        Empty
};
public class BoardScript : MonoBehaviour
{
    private struct SelectedItems
    {
        public SelectedItems(GameObject _obj, Material _objMaterial)
        {
            obj = _obj;
            objMaterial =  _objMaterial;
        }
        public GameObject obj;
        public Material objMaterial;
    };
    [SerializeField] private GameObject EnemyAIobj;
    [SerializeField] private Material onSelectMaterial;
    [SerializeField] private GameObject whiteStonesObj;
    private GameObject[,] Grid = new GameObject[8,8];
    private List<SelectedItems> CurSelected = new List<SelectedItems>();
    private GameObject selectedStone;
    private bool CanAttack = true;
    private bool IsAttackCombo = false;
    private void Awake()
    {
        for (int i = 0; i < Mathf.Sqrt(Grid.Length); i++)
        {
            for (int j = 0; j < Mathf.Sqrt(Grid.Length); j++)
            {
                Transform temp = this.transform.GetChild(i);
                Grid[i, j] = temp.GetChild(j).gameObject;
            }
        }
    }
    public void OnSelect(int x, int z, GameObject selectedObj)
    {
        ClearSelection();
        
        CurSelected.Add(new SelectedItems(selectedObj, selectedObj.GetComponent<Renderer>().material));
        selectedStone = selectedObj;
        selectedObj.GetComponent<Renderer>().material = onSelectMaterial;

        if (z + 1 >= 0 && z + 1 < Mathf.Sqrt(Grid.Length))
        {
            //Check movemnt Up-Right
            if (x + 1 >= 0 && x + 1 < Mathf.Sqrt(Grid.Length))
            {
                if (!Grid[x + 1, z + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CurSelected.Add(new SelectedItems(Grid[x + 1, z + 1], Grid[x + 1, z + 1].GetComponent<Renderer>().material));
                    Grid[x + 1, z + 1].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x + 1, z + 1].GetComponent<SlotScript>().SetSelection(true);
                }
            }
            //Check movemnt Up-Right
            if (x - 1 >= 0 && x - 1 < Mathf.Sqrt(Grid.Length))
            {
                if (!Grid[x - 1, z + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CurSelected.Add(new SelectedItems(Grid[x - 1, z + 1], Grid[x - 1, z + 1].GetComponent<Renderer>().material));
                    Grid[x - 1, z + 1].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x - 1, z + 1].GetComponent<SlotScript>().SetSelection(true);
                }
            }
        }
        SelectionToAttack(x, z);
    }
    public void SelectionToAttack(int x, int z)
    {
        CanAttack = false;
        int MovesCount = CurSelected.Count;
        //Check Upper
        if (z + 2 >= 0 && z + 2 < Mathf.Sqrt(Grid.Length))
        {
            if (x + 2 >= 0 && x + 2 < Mathf.Sqrt(Grid.Length) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelected.Add(new SelectedItems(Grid[x + 2, z + 2], Grid[x + 2, z + 2].GetComponent<Renderer>().material));
                Grid[x + 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z + 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
            if (x - 2 >= 0 && x - 2 < Mathf.Sqrt(Grid.Length) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelected.Add(new SelectedItems(Grid[x - 2, z + 2], Grid[x - 2, z + 2].GetComponent<Renderer>().material));
                Grid[x - 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x - 2, z + 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
        }
        //Check Lower
        if (z - 2 >= 0 && z - 2 < Mathf.Sqrt(Grid.Length))
        {
            if (x + 2 >= 0 && x + 2 < Mathf.Sqrt(Grid.Length) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelected.Add(new SelectedItems(Grid[x + 2, z - 2], Grid[x + 2, z - 2].GetComponent<Renderer>().material));
                Grid[x + 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z - 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
            if (x - 2 >= 0 && x - 2 < Mathf.Sqrt(Grid.Length) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelected.Add(new SelectedItems(Grid[x - 2, z - 2], Grid[x - 2, z - 2].GetComponent<Renderer>().material));
                Grid[x - 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x - 2, z - 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
        }

        if (CanAttack) ClearSelection(0, MovesCount);
    }
    public void ClearSelection()
    {
        if (CurSelected.Count > 0)
        {
            for (int i = 0; i < CurSelected.Count; i++)
            {
                if (CurSelected[i].obj) CurSelected[i].obj.GetComponent<Renderer>().material = CurSelected[i].objMaterial;
                if (CurSelected[i].obj != selectedStone) CurSelected[i].obj.GetComponent<SlotScript>().SetSelection(false);
            }
        }
        CurSelected.Clear();
    }
    public void ClearSelection(int start, int end)
    {
        if (CurSelected.Count > 0)
        {
            for (int i = start; i < end; i++)
            {
                CurSelected[i].obj.GetComponent<Renderer>().material = CurSelected[i].objMaterial;
                if (CurSelected[i].obj != selectedStone) CurSelected[i].obj.GetComponent<SlotScript>().SetSelection(false);
            }
        }
        CurSelected.RemoveRange(start, end);
    }
    public void MoveSelectedStones(int x, int z)
    {
        int tempX = (int)selectedStone.transform.position.x;
        int tempZ = (int)selectedStone.transform.position.z;
        SetUnOcupied((int)selectedStone.transform.position.x, (int)selectedStone.transform.position.z);
        selectedStone.transform.position = new Vector3(x, 0.2f, z);
        if (tempZ < z)
        {
            if (tempX < x)
            {
                for (; tempX < x; tempX++, tempZ++)
                {
                    if (Grid[tempX, tempZ].GetComponent<SlotScript>().IsOcupied())
                    {
                        EnemyAIobj.GetComponent<EnemyAI>().KillStones(tempX, tempZ);
                        SetUnOcupied(tempX, tempZ);
                        SelectionToAttack(x, z);
                    }
                }
            }
            else if (tempX > x)
            {
                for (; tempX > x; tempX--, tempZ++)
                {
                    if (Grid[tempX, tempZ].GetComponent<SlotScript>().IsOcupied())
                    {
                        EnemyAIobj.GetComponent<EnemyAI>().KillStones(tempX, tempZ);
                        SetUnOcupied(tempX, tempZ);
                        SelectionToAttack(x, z);
                    }
                }
            }
        }
        else
        {
            if (tempX < x)
            {
                for (; tempX < x; tempX++, tempZ--)
                {
                    if (Grid[tempX, tempZ].GetComponent<SlotScript>().IsOcupied())
                    {
                        EnemyAIobj.GetComponent<EnemyAI>().KillStones(tempX, tempZ);
                        SetUnOcupied(tempX, tempZ);
                        SelectionToAttack(x, z);
                    }
                }
            }
            else if (tempX > x)
            {
                for (; tempX > x; tempX--, tempZ--)
                {
                    if (Grid[tempX, tempZ].GetComponent<SlotScript>().IsOcupied())
                    {
                        EnemyAIobj.GetComponent<EnemyAI>().KillStones(tempX, tempZ);
                        SetUnOcupied(tempX, tempZ);
                        SelectionToAttack(x, z);
                    }
                }
            }
        }
        SetOcupied((int)selectedStone.transform.position.x, (int)selectedStone.transform.position.z, Color.White);
        
        if (!CanAttack)
        {
            EnemyAIobj.GetComponent<EnemyAI>().OpponentMove();
            IsAttackCombo = false;
        }
    }
    public void SetOcupied(int x, int z, Color color)
    {
        Grid[x, z].GetComponent<SlotScript>().SetOcupied(true, color);
    }
    public void SetUnOcupied(int x, int z)
    {
        Grid[x, z].GetComponent<SlotScript>().SetOcupied(false, Color.Empty);
    }
    public bool OnSelectEnemy(int x, int z, GameObject selectedObj, int type)
    {
        ClearSelection();
        
        CurSelected.Add(new SelectedItems(selectedObj, selectedObj.GetComponent<Renderer>().material));
        selectedStone = selectedObj;
        selectedObj.GetComponent<Renderer>().material = onSelectMaterial;


        if (z - 1 >= 0 && z - 1 < Mathf.Sqrt(Grid.Length))
        {
            if (type == 1 && x + 1 >= 0 && x + 1 < Mathf.Sqrt(Grid.Length))
            {
                if (!Grid[x + 1, z - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CurSelected.Add(new SelectedItems(Grid[x + 1, z - 1], Grid[x + 1, z - 1].GetComponent<Renderer>().material));
                    Grid[x + 1, z - 1].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x + 1, z - 1].GetComponent<SlotScript>().SetSelection(true);
                    SetUnOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z);
                    selectedObj.transform.position = new Vector3(x + 1, 0.2f, z - 1);
                    SetOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z, Color.Black);
                    
                    ClearSelection();
                    return true;
                }
            }
            if (type == 2 && x - 1 >= 0 && x - 1 < Mathf.Sqrt(Grid.Length))
            {
                if (!Grid[x - 1, z - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CurSelected.Add(new SelectedItems(Grid[x - 1, z - 1], Grid[x - 1, z - 1].GetComponent<Renderer>().material));
                    Grid[x - 1, z - 1].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x - 1, z - 1].GetComponent<SlotScript>().SetSelection(true);
                    SetUnOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z);
                    selectedObj.transform.position = new Vector3(x - 1, 0.2f, z - 1);
                    SetOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z, Color.Black);
                    
                    ClearSelection();
                    return true;
                }
            }
        }
        return false;
    }
    public bool CheckEnemyAttack(GameObject enemyStone)
    {
        int x = (int)enemyStone.transform.position.x;
        int z = (int)enemyStone.transform.position.z;

        if (z + 2 >= 0 && z + 2 < Mathf.Sqrt(Grid.Length))
        {
            if (x + 2 >= 0 && x + 2 < Mathf.Sqrt(Grid.Length) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, 1, enemyStone);
                Debug.Log("TRUE");
                return true;
            }
            if (x - 2 >= 0 && x - 2 < Mathf.Sqrt(Grid.Length) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, 2, enemyStone);
                Debug.Log("TRUE");
                return true;
            }
        }
        if (z - 2 >= 0 && z - 2 < Mathf.Sqrt(Grid.Length))
        {
            if (x + 2 >= 0 && x + 2 < Mathf.Sqrt(Grid.Length) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, 3, enemyStone);
                Debug.Log("TRUE");
                return true;
            }
            if (x - 2 >= 0 && x - 2 < Mathf.Sqrt(Grid.Length) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, 4, enemyStone);
                Debug.Log("TRUE");
                return true;
            }
        }
        Debug.Log("FALSE");
        return false;
    }
    private void EnemyAttack(int x, int z, int type, GameObject enemyStone)
    {
        int tempX = (int)enemyStone.transform.position.x;
        int tempZ = (int)enemyStone.transform.position.z;

        SetUnOcupied(tempX, tempZ);

        if (type == 1)
        {
            enemyStone.transform.position = new Vector3(tempX + 2, 0.2f, tempZ + 2);
            whiteStonesObj.GetComponent<WhiteStonesHandle>().FindAndDelete(tempX + 1, tempZ + 1);
            SetOcupied(tempX + 2, tempZ + 2, Color.Black);
        }
        else if (type == 2)
        {
            enemyStone.transform.position = new Vector3(tempX - 2, 0.2f, tempZ + 2);
            whiteStonesObj.GetComponent<WhiteStonesHandle>().FindAndDelete(tempX - 1, tempZ + 1);
            SetOcupied(tempX - 2, tempZ + 2, Color.Black);
        }
        else if (type == 3)
        {
            enemyStone.transform.position = new Vector3(tempX + 2, 0.2f, tempZ - 2);
            whiteStonesObj.GetComponent<WhiteStonesHandle>().FindAndDelete(tempX + 1, tempZ - 1);
            SetOcupied(tempX + 2, tempZ - 2, Color.Black);
        }
        else if (type == 4)
        {
            enemyStone.transform.position = new Vector3(tempX - 2, 0.2f, tempZ - 2);
            whiteStonesObj.GetComponent<WhiteStonesHandle>().FindAndDelete(tempX - 1, tempZ - 1);
            SetOcupied(tempX - 2, tempZ - 2, Color.Black);
        }
    }
    public bool IsCombo()
    {
        return IsAttackCombo;
    }
}