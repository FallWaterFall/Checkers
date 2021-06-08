using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color
{
    Empty,
    White,
    Black
};
public struct SelectedItems
    {
        public SelectedItems(GameObject _obj, Material _objMaterial)
        {
            obj = _obj;
            objMaterial =  _objMaterial;
        }
        public GameObject obj;
        public Material objMaterial;
    };
public class BoardScript : MonoBehaviour
{
    [SerializeField] private GameObject EnemyAIobj;
    [SerializeField] private Material onSelectMaterial;
    [SerializeField] private GameObject whiteStonesObj;
    private WhiteStonesHandle WSH;
    private EnemyAI EAI;
    private GameObject[,] Grid = new GameObject[8,8];
    private int BoardLength;
    private List<SelectedItems> CurSelectedCells = new List<SelectedItems>();
    private GameObject selectedStone;
    private bool CanAttack = true;
    private bool IsAttackCombo = false;
    private bool CanSelectAnother = true;
    private void Awake()
    {
        BoardLength =(int)Mathf.Sqrt(Grid.Length);
        for (int i = 0; i < BoardLength; i++)
        {
            for (int j = 0; j < BoardLength; j++)
            {
                Transform temp = this.transform.GetChild(i);
                Grid[i, j] = temp.GetChild(j).gameObject;
            }
        }
        WSH = whiteStonesObj.GetComponent<WhiteStonesHandle>();
        EAI = EnemyAIobj.GetComponent<EnemyAI>();
    }
    public void SelectCells(int x, int z)
    {
        ClearSelection();

        if (IsZExist(z + 1))
        {
            //Check movemnt Up-Right
            if (IsXExist(x + 1))
            {
                if (!Grid[x + 1, z + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CurSelectedCells.Add(new SelectedItems(Grid[x + 1, z + 1], Grid[x + 1, z + 1].GetComponent<Renderer>().material));
                    Grid[x + 1, z + 1].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x + 1, z + 1].GetComponent<SlotScript>().SetSelection(true);
                }
            }
            //Check movemnt Up-Right
            if (IsXExist(x - 1))
            {
                if (!Grid[x - 1, z + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CurSelectedCells.Add(new SelectedItems(Grid[x - 1, z + 1], Grid[x - 1, z + 1].GetComponent<Renderer>().material));
                    Grid[x - 1, z + 1].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x - 1, z + 1].GetComponent<SlotScript>().SetSelection(true);
                }
            }
        }
        SelectCellsToAttack(x, z);
    }
    public bool SelectCellsToAttack(int x, int z)
    {
        CanAttack = false;
        IsAttackCombo = false;
        int MovesCount = CurSelectedCells.Count;

        if (IsZExist(z + 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z + 2], Grid[x + 2, z + 2].GetComponent<Renderer>().material));
                Grid[x + 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z + 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z + 2], Grid[x - 2, z + 2].GetComponent<Renderer>().material));
                Grid[x - 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x - 2, z + 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
        }
        //Check Lower
        if (IsZExist(z - 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z - 2], Grid[x + 2, z - 2].GetComponent<Renderer>().material));
                Grid[x + 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z - 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.Black && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z - 2], Grid[x - 2, z - 2].GetComponent<Renderer>().material));
                Grid[x - 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x - 2, z - 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
        }
        if (CanAttack)
        {
            CanSelectAnother = false;
            ClearSelection(0, MovesCount);
            return true;
        } else return false;

    }
    public void MoveSelectedStone(int endX, int endZ)
    {
        Vector2Int startV = WSH.GetSelectedStone();
        
        WSH.MoveStone(endX, endZ);

        DestroyEnemyStonesOnTheWay(startV.x, startV.y, endX, endZ);
        
        SetOcupied(endX, endZ, Color.White);
        
        ClearSelection();

        if (IsAttackCombo)
        {
            Vector2Int temp = WSH.GetSelectedStone();
            CanSelectAnother = false;
            SelectCellsToAttack(temp.x, temp.y);
        }
        if (IsAttackCombo == false) CanSelectAnother = true;

        if (CurSelectedCells.Count <= 0)
        {
            EAI.OpponentMove();

            WSH.FindTarget();
        }
    }
    private void DestroyEnemyStonesOnTheWay(int startX, int startZ, int endX, int endZ)
    {
        if (startX < endX && startZ < endZ)
        {
            for (; startX < endX; startX++, startZ++)
            {
                if (Grid[startX, startZ].GetComponent<SlotScript>().IsOcupied())
                {
                    EAI.KillStones(startX, startZ);
                }
            }
            return;
        }
        if (startX > endX && startZ < endZ)
        {
            for (; startX > endX; startX--, startZ++)
            {
                if (Grid[startX, startZ].GetComponent<SlotScript>().IsOcupied())
                {
                    EAI.KillStones(startX, startZ);
                }
            }
            return;
        }
        if (startX < endX && startZ > endZ)
        {
            for (; startX < endX; startX++, startZ--)
            {
                if (Grid[startX, startZ].GetComponent<SlotScript>().IsOcupied())
                {
                    EAI.KillStones(startX, startZ);
                }
            }
            return;
        }
        if (startX > endX && startZ > endZ)
        {
            for (; startX > endX; startX--, startZ--)
            {
                if (Grid[startX, startZ].GetComponent<SlotScript>().IsOcupied())
                {
                    EAI.KillStones(startX, startZ);
                }
            }
            return;
        }
    }
    private bool IsCellExist(int x, int z)
    {
        if (IsXExist(x) && IsZExist(z))
            return true;
        else
            return false;
    }
    private bool IsXExist(int x)
    {
        if (x >= 0 && x < BoardLength)
            return true;
        else
            return false;
    }
    private bool IsZExist(int z)
    {
        if (z >= 0 && z < BoardLength)
            return true;
        else
            return false;
    }
    public void ClearSelection()
    {
        if (CurSelectedCells.Count > 0)
        {
            for (int i = 0; i < CurSelectedCells.Count; i++)
            {
                if (CurSelectedCells[i].obj) CurSelectedCells[i].obj.GetComponent<Renderer>().material = CurSelectedCells[i].objMaterial;
                if (CurSelectedCells[i].obj != selectedStone) CurSelectedCells[i].obj.GetComponent<SlotScript>().SetSelection(false);
            }
        }
        CurSelectedCells.Clear();
    }
    public void ClearSelection(int start, int end)
    {
        if (CurSelectedCells.Count > 0)
        {
            for (int i = start; i < end; i++)
            {
                CurSelectedCells[i].obj.GetComponent<Renderer>().material = CurSelectedCells[i].objMaterial;
                if (CurSelectedCells[i].obj != selectedStone) CurSelectedCells[i].obj.GetComponent<SlotScript>().SetSelection(false);
            }
        }
        CurSelectedCells.RemoveRange(start, end);
    }
    public void SetOcupied(int x, int z, Color color)
    {
        Grid[x, z].GetComponent<SlotScript>().SetOcupied(true, color);
    }
    public void SetUnOcupied(int x, int z)
    {
        Grid[x, z].GetComponent<SlotScript>().SetOcupied(false, Color.Empty);
    }
    public bool CanSelect()
    {
        return CanSelectAnother;
    }
    //ENEMY
    public bool EnemyMove(int x, int z, GameObject selectedObj, int type)
    {
        if (IsZExist(z - 1))
        {
            if (type == 1 && IsXExist(x + 1))
            {
                if (!Grid[x + 1, z - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z);
                    selectedObj.transform.position = new Vector3(x + 1, 0.2f, z - 1);
                    SetOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z, Color.Black);
                    
                    return true;
                }
            }
            if (type == 2 && IsXExist(x - 1))
            {
                if (!Grid[x - 1, z - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z);
                    selectedObj.transform.position = new Vector3(x - 1, 0.2f, z - 1);
                    SetOcupied((int)selectedObj.transform.position.x, (int)selectedObj.transform.position.z, Color.Black);
                    
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

        if (IsZExist(z + 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 1);
                Debug.Log("TRUE");
                return true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 2);
                Debug.Log("TRUE");
                return true;
            }
        }
        if (IsZExist(z - 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 3);
                Debug.Log("TRUE");
                return true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == Color.White && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 4);
                Debug.Log("TRUE");
                return true;
            }
        }
        Debug.Log("FALSE");
        return false;
    }
    private void EnemyAttack(int x, int z, GameObject enemyStone, int type)
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
}