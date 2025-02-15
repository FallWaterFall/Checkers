using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StonesColor
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
    [SerializeField] private Material onSelectChoiseMaterial;
    [SerializeField] private GameObject allyStonesObj;
    [SerializeField] private GameObject MenuCanvas;
    public float MoveSpeed = 0.51f;
    private bool CanvasActivity = false;
    private AllyStonesHandle ASH;
    private EnemyAI EAI;
    private GameObject[,] Grid = new GameObject[8,8];
    private int BoardLength;
    private List<SelectedItems> CurSelectedCells = new List<SelectedItems>();
    private GameObject selectedStone;
    private bool CanAttack = true;
    private bool IsAttackCombo = false;
    private bool CanSelectAnother = true;
    private bool IsSelectKing = false;
    private List<GameObject> CanSelectStone = new List<GameObject>();
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
        ASH = allyStonesObj.GetComponent<AllyStonesHandle>();
        EAI = EnemyAIobj.GetComponent<EnemyAI>();
        MenuCanvas.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowCanvas();

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
    public void SelectCellsForKing(int x, int z)
    {
        ClearSelection();

        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (IsCellExist(leftX, tempZ) && Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied() && IsCellExist(leftX - 1, tempZ - 1) && Grid[leftX - 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied()) break;

            if (IsXExist(leftX) && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }
        }

        for (int rightX = x + 1, tempZ = z - 1; tempZ >= 0; rightX++, tempZ--)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;
            
            if (IsCellExist(rightX, tempZ) && Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied() && IsCellExist(rightX + 1, tempZ - 1) && Grid[rightX + 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied()) break;

            if (IsXExist(rightX) && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }
        }

        for (int leftX = x - 1, tempZ = z + 1; tempZ < 8; leftX--, tempZ++)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;
            
            if (IsCellExist(leftX, tempZ) && Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied() && IsCellExist(leftX - 1, tempZ + 1) && Grid[leftX - 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied()) break;

            if (IsXExist(leftX) && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }
        }

        for (int rightX = x + 1, tempZ = z + 1; tempZ < 8; rightX++, tempZ++)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;
            
            if (IsCellExist(rightX, tempZ) && Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied() && IsCellExist(rightX + 1, tempZ + 1) && Grid[rightX + 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied()) break;

            if (IsXExist(rightX) && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }
        }
    }
    public bool SerchCellsForAttack(int x, int z)
    {
        CanAttack = false;
        if (IsZExist(z + 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z + 2], Grid[x + 2, z + 2].GetComponent<Renderer>().material));
                Grid[x + 2, z + 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z + 2], Grid[x - 2, z + 2].GetComponent<Renderer>().material));
                Grid[x - 2, z + 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
        }
        //Check Lower
        if (IsZExist(z - 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z - 2], Grid[x + 2, z - 2].GetComponent<Renderer>().material));
                Grid[x + 2, z - 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z - 2], Grid[x - 2, z - 2].GetComponent<Renderer>().material));
                Grid[x - 2, z - 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
        }

        if (CanAttack)
        {
            CanSelectAnother = false;
            return true;
        } else return false;
    }
    public bool SerchCellsForKingAttack(int x, int z)
    {
        bool CanAttackHere = false;
        CanAttack = false;
        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (!IsCellExist(leftX, tempZ)) break;
            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(leftX - 1, tempZ - 1) && !Grid[leftX - 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }
        CanAttackHere = false;
        for (int rightX = x + 1, tempZ = z - 1; tempZ >= 0; rightX++, tempZ--)
        {
            if (!IsCellExist(rightX, tempZ)) break;
            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(rightX + 1, tempZ - 1) && !Grid[rightX + 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }
        CanAttackHere = false;
        for (int leftX = x - 1, tempZ = z + 1; tempZ >= 0; leftX--, tempZ++)
        {
            if (!IsCellExist(leftX, tempZ)) break;
            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(leftX - 1, tempZ + 1) && !Grid[leftX - 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }
        CanAttackHere = false;
        for (int rightX = x + 1, tempZ = z + 1; tempZ >= 0; rightX++, tempZ++)
        {
            if (!IsCellExist(rightX, tempZ)) break;
            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(rightX + 1, tempZ + 1) && !Grid[rightX + 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }

        if (CanAttack)
        {
            CanSelectAnother = false;
            return true;
        } else return false;
    }
    public bool SelectCellsToAttack(int x, int z)
    {
        IsSelectKing = false;
        CanAttack = false;
        IsAttackCombo = false;
        int MovesCount = CurSelectedCells.Count;

        if (IsZExist(z + 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z + 2], Grid[x + 2, z + 2].GetComponent<Renderer>().material));
                Grid[x + 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z + 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
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
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z - 2], Grid[x + 2, z - 2].GetComponent<Renderer>().material));
                Grid[x + 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z - 2].GetComponent<SlotScript>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
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
    public bool SelectCellsToKingAttack(int x, int z)
    {
        ClearSelection();

        bool CanAttackHere = false;
        CanAttack = false;
        IsAttackCombo = false;
        IsSelectKing = false;

        CanAttackHere = false;
        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (!IsCellExist(leftX, tempZ)) break;
            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }

            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(leftX - 1, tempZ - 1) && !Grid[leftX - 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }
        CanAttackHere = false;
        for (int rightX = x + 1, tempZ = z - 1; tempZ >= 0; rightX++, tempZ--)
        {
            if (!IsCellExist(rightX, tempZ)) break;
            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }

            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(rightX + 1, tempZ - 1) && !Grid[rightX + 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }
        CanAttackHere = false;
        for (int leftX = x - 1, tempZ = z + 1; tempZ < 8; leftX--, tempZ++)
        {
            if (!IsCellExist(leftX, tempZ)) break;
            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }

            if (Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(leftX - 1, tempZ + 1) && !Grid[leftX - 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }
        CanAttackHere = false;
        for (int rightX = x + 1, tempZ = z + 1; tempZ < 8; rightX++, tempZ++)
        {
            if (!IsCellExist(rightX, tempZ)) break;
            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White) break;

            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<SlotScript>().SetSelection(true);
            }

            if (Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black)
            { 
                if (IsCellExist(rightX + 1, tempZ + 1) && !Grid[rightX + 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }

        if (CanAttack)
        {
            IsAttackCombo = true;
            IsSelectKing = true;
            CanSelectAnother = false;
            return true;
        } else return false;
    }
    public IEnumerator MoveSelectedStone(int endX, int endZ)
    {
        Vector2Int startV = ASH.GetSelectedStone();
        
        ASH.MoveStone(endX, endZ);

        SetCanSelect(false);
        ClearSelection();
        CanSelectStone.Clear();
        
        yield return new WaitForSeconds(MoveSpeed);

        DestroyEnemyStonesOnTheWay(startV.x, startV.y, endX, endZ);
        
        SetOcupied(endX, endZ, StonesColor.White);

        if (IsAttackCombo)
        {
            Vector2Int temp = ASH.GetSelectedStone();
            if (IsSelectKing) SelectCellsToKingAttack(temp.x, temp.y);
            else SelectCellsToAttack(temp.x, temp.y);
        }

        selectedStone = null;

        if (CurSelectedCells.Count <= 0)
        {
            SetCanSelect(false);
            StartCoroutine(EAI.OpponentMove());
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
    public GameObject GetCell(int x, int z)
    {
        return Grid[x, z];
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
    public void SetOcupied(int x, int z, StonesColor color)
    {
        Grid[x, z].GetComponent<SlotScript>().SetOcupied(true, color);
    }
    public void SetUnOcupied(int x, int z)
    {
        Grid[x, z].GetComponent<SlotScript>().SetOcupied(false, StonesColor.Empty);
    }
    public bool CanSelect()
    {
        return CanSelectAnother;
    }
    public void SetCanSelect(bool b)
    {
        CanSelectAnother = b;
    }
    public bool IsInCanSelectList(GameObject obj)
    {
        if (CanSelectStone.Count > 0)
        {
            for (int i = 0; i < CanSelectStone.Count; i++)
            {
                if (CanSelectStone[i] == obj) return true;
            }
        }
        return false;
    }
    public void AddInCanSelectList(GameObject obj)
    {
        CanSelectStone.Add(obj);
    }
    public void SetIsSelectKing(bool b)
    {
        IsSelectKing = b;
    }
    public void FindTarget()
    {
        CanSelectStone.Clear();
        ASH.FindTarget();
        if (CanSelectStone.Count == 0) CanSelectAnother = true;
    }
    //ENEMY////////////////////////////////////////
    public bool EnemyMove(int x, int z, int type)
    {
        if (IsZExist(z - 1))
        {
            if (type == 1 && IsXExist(x + 1))
            {
                if (!Grid[x + 1, z - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied(x, z);
                    EAI.MakeMoveAnim(x + 1, z - 1);
                    SetOcupied(x + 1, z - 1, StonesColor.Black);

                    return true;
                }
            }
            if (type == 2 && IsXExist(x - 1))
            {
                if (!Grid[x - 1, z - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied(x, z);
                    EAI.MakeMoveAnim(x - 1, z - 1);
                    SetOcupied(x - 1, z - 1, StonesColor.Black);
                    
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
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White && !Grid[x + 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 1);
                return true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White && !Grid[x - 2, z + 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 2);
                return true;
            }
        }
        if (IsZExist(z - 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White && !Grid[x + 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 3);
                return true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White && !Grid[x - 2, z - 2].GetComponent<SlotScript>().IsOcupied())
            {
                EnemyAttack(x, z, enemyStone, 4);
                return true;
            }
        }
        return false;
    }
    private void EnemyAttack(int x, int z, GameObject enemyStone, int type)
    {
        int tempX = (int)enemyStone.transform.position.x;
        int tempZ = (int)enemyStone.transform.position.z;

        int animX = 0, animZ = 0;

        SetUnOcupied(tempX, tempZ);
        if (type == 1)
        {
            animX = tempX + 2; animZ = tempZ + 2;
        }
        else if (type == 2)
        {
            animX = tempX - 2; animZ = tempZ + 2;
        }
        else if (type == 3)
        {
            animX = tempX + 2; animZ = tempZ - 2;
        }
        else if (type == 4)
        {
            animX = tempX - 2; animZ = tempZ - 2;
        }
        EAI.MakeMoveAnim(animX, animZ, enemyStone);
        StartCoroutine(ASH.FindAndDelete(tempX + (animX - tempX) / 2, tempZ + (animZ - tempZ) / 2));
        SetOcupied(animX, animZ, StonesColor.Black);
    }
    public bool CheckEnemyKingAttack(GameObject enemyKing)
    {
        int x = (int)enemyKing.transform.position.x;
        int z = (int)enemyKing.transform.position.z;

        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White)
            {
                if (IsCellExist(leftX - 1, tempZ - 1) && !Grid[leftX - 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied(x, z);
                    EAI.MakeMoveAnim(leftX - 1, tempZ - 1, enemyKing);
                    SetOcupied(leftX - 1, tempZ - 1, StonesColor.Black);
                    StartCoroutine(ASH.FindAndDelete(leftX, tempZ));
                    return true;
                } else break;
            }
        }

        for (int rightX = x + 1, tempZ = z - 1; tempZ >= 0; rightX++, tempZ--)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White)
            {
                if (IsCellExist(rightX + 1, tempZ - 1) && !Grid[rightX + 1, tempZ - 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied(x, z);
                    EAI.MakeMoveAnim(rightX + 1, tempZ - 1, enemyKing);
                    SetOcupied(rightX + 1, tempZ - 1, StonesColor.Black);
                    StartCoroutine(ASH.FindAndDelete(rightX, tempZ));
                    return true;
                } else break;
            }
        }

        for (int leftX = x - 1, tempZ = z + 1; tempZ < 8; leftX--, tempZ++)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White)
            {
                if (IsCellExist(leftX - 1, tempZ + 1) && !Grid[leftX - 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied(x, z);
                    EAI.MakeMoveAnim(leftX - 1, tempZ + 1, enemyKing);
                    SetOcupied(leftX - 1, tempZ + 1, StonesColor.Black);
                    StartCoroutine(ASH.FindAndDelete(leftX, tempZ));
                    return true;
                } else break;
            }
        }

        for (int rightX = x + 1, tempZ = z + 1; tempZ < 8; rightX++, tempZ++)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.White)
            {
                if (IsCellExist(rightX + 1, tempZ + 1) && !Grid[rightX + 1, tempZ + 1].GetComponent<SlotScript>().IsOcupied())
                {
                    SetUnOcupied(x, z);
                    EAI.MakeMoveAnim(rightX + 1, tempZ + 1, enemyKing);
                    SetOcupied(rightX + 1, tempZ + 1, StonesColor.Black);
                    StartCoroutine(ASH.FindAndDelete(rightX, tempZ));
                    return true;
                } else break;
            }
        }
        return false;
    }
    public bool MoveEnemyKing(int x, int z, GameObject enemyKing)
    {
        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;
            
            if (IsXExist(leftX) && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                SetUnOcupied(x, z);
                EAI.MakeMoveAnim(leftX, tempZ, enemyKing);
                SetOcupied(leftX, tempZ, StonesColor.Black);
                StartCoroutine(ASH.FindAndDelete(leftX, tempZ));
                return true;
            }
        }

        for (int rightX = x + 1, tempZ = z - 1; tempZ >= 0; rightX++, tempZ--)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(rightX) && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                SetUnOcupied(x, z);
                EAI.MakeMoveAnim(rightX, tempZ, enemyKing);
                SetOcupied(rightX, tempZ, StonesColor.Black);
                StartCoroutine(ASH.FindAndDelete(rightX, tempZ));
                return true;
            }
        }

        for (int leftX = x - 1, tempZ = z + 1; tempZ < 8; leftX--, tempZ++)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(leftX) && !Grid[leftX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                SetUnOcupied(x, z);
                EAI.MakeMoveAnim(leftX, tempZ, enemyKing);
                SetOcupied(leftX, tempZ, StonesColor.Black);
                StartCoroutine(ASH.FindAndDelete(leftX, tempZ));
                return true;
            }
        }

        for (int rightX = x + 1, tempZ = z + 1; tempZ < 8; rightX++, tempZ++)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<SlotScript>().WhatIsColor() == StonesColor.Black) break;

            if (IsXExist(rightX) && !Grid[rightX, tempZ].GetComponent<SlotScript>().IsOcupied())
            {
                SetUnOcupied(x, z);
                EAI.MakeMoveAnim(rightX, tempZ, enemyKing);
                SetOcupied(rightX, tempZ, StonesColor.Black);
                StartCoroutine(ASH.FindAndDelete(rightX, tempZ));
                return true;
            }
        }
        return false;
    }
    //Level control////////////////////////////////
    public void EndGame(bool win)
    {
        ClearSelection();
        SetCanSelect(false);
        MenuCanvas.SetActive(true);
        if (win)
        {
            SaveData data = SaveSystem.Load();
            data.moneyAmount += 10;
            SaveSystem.Save(data);
        }
    }
    public void ShowCanvas()
    {
        CanvasActivity = !CanvasActivity;
        MenuCanvas.SetActive(CanvasActivity);
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}