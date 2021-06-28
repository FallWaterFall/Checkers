using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardServer : MonoBehaviour
{
    [SerializeField] private Material onSelectMaterial;
    [SerializeField] private Material onSelectChoiseMaterial;
    [SerializeField] private GameObject StonesObj;
    [SerializeField] private GameObject WhiteCellObj;
    public float MoveSpeed = 0.52f; //Dont set lower (MoveSpeed * x + 0.2f)
    private StonesHandle SH;
    private GameObject[,] Grid = new GameObject[8,8];
    private int BoardLength;
    private List<SelectedItems> CurSelectedCells = new List<SelectedItems>();
    private GameObject selectedStone;
    private bool CanSelectAnother = true;
    private bool CanAttack = true;
    private bool IsAttackCombo = false;
    private bool IsSelectKing = false;
    private List<GameObject> CanSelectStone = new List<GameObject>();
    private StonesColor WhoseTurn = StonesColor.White;
    private void Awake()
    {
        BoardLength =(int)Mathf.Sqrt(Grid.Length);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform temp = this.transform.GetChild(i);
            for (int j = 0; j < temp.childCount; j++)
            {
                Grid[i, j] = temp.GetChild(j).gameObject;
            }
        }
    }
    private void Start()
    {
        SH = GameObject.Find("Pivot").GetComponent<StonesHandle>();
    }
    public void SelectCells(int x, int z)
    {
        ClearSelection();

        int tZ;
        if (WhoseTurn == StonesColor.White) tZ = z + 1;
        else tZ = z - 1;

        if (IsZExist(tZ))
        {
            //Check movemnt Up-Right
            if (IsCellExist(x + 1, tZ))
            {
                if (!Grid[x + 1, tZ].GetComponent<CellScriptServer>().IsOcupied())
                {
                    CurSelectedCells.Add(new SelectedItems(Grid[x + 1, tZ], Grid[x + 1, tZ].GetComponent<Renderer>().material));
                    Grid[x + 1, tZ].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x + 1, tZ].GetComponent<CellScriptServer>().SetSelection(true);
                }
            }
            //Check movemnt Up-Right
            if (IsCellExist(x - 1, tZ))
            {
                if (!Grid[x - 1, tZ].GetComponent<CellScriptServer>().IsOcupied())
                {
                    CurSelectedCells.Add(new SelectedItems(Grid[x - 1, tZ], Grid[x - 1, tZ].GetComponent<Renderer>().material));
                    Grid[x - 1, tZ].GetComponent<Renderer>().material = onSelectMaterial;
                    Grid[x - 1, tZ].GetComponent<CellScriptServer>().SetSelection(true);
                }
            }
        }
        SelectCellsToAttack(x, z);
    }
    public void SelectCellsForKing(int x, int z)
    {
        ClearSelection();

        StonesColor CurAllyColor = StonesColor.Empty;
        if (WhoseTurn == StonesColor.White) CurAllyColor = StonesColor.White;
        else CurAllyColor = StonesColor.Black;

        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            if (IsCellExist(leftX, tempZ) && Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied() && IsCellExist(leftX - 1, tempZ - 1) && Grid[leftX - 1, tempZ - 1].GetComponent<CellScriptServer>().IsOcupied()) break;

            if (IsXExist(leftX) && !Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }
        }

        for (int rightX = x + 1, tempZ = z - 1; tempZ >= 0; rightX++, tempZ--)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;
            
            if (IsCellExist(rightX, tempZ) && Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied() && IsCellExist(rightX + 1, tempZ - 1) && Grid[rightX + 1, tempZ - 1].GetComponent<CellScriptServer>().IsOcupied()) break;

            if (IsXExist(rightX) && !Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }
        }

        for (int leftX = x - 1, tempZ = z + 1; tempZ < 8; leftX--, tempZ++)
        {
            if (IsXExist(leftX) && Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;
            
            if (IsCellExist(leftX, tempZ) && Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied() && IsCellExist(leftX - 1, tempZ + 1) && Grid[leftX - 1, tempZ + 1].GetComponent<CellScriptServer>().IsOcupied()) break;

            if (IsXExist(leftX) && !Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }
        }

        for (int rightX = x + 1, tempZ = z + 1; tempZ < 8; rightX++, tempZ++)
        {
            if (IsXExist(rightX) && Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;
            
            if (IsCellExist(rightX, tempZ) && Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied() && IsCellExist(rightX + 1, tempZ + 1) && Grid[rightX + 1, tempZ + 1].GetComponent<CellScriptServer>().IsOcupied()) break;

            if (IsXExist(rightX) && !Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }
        }
    }
    public bool SerchCellsForAttack(int x, int z)
    {
        StonesColor preferredColor = StonesColor.Empty;
        if (WhoseTurn == StonesColor.White) preferredColor = StonesColor.Black;
        else preferredColor = StonesColor.White;

        CanAttack = false;

        if (IsZExist(z + 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x + 2, z + 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z + 2], Grid[x + 2, z + 2].GetComponent<Renderer>().material));
                if (IsInCurSelectedCells(Grid[x + 2, z + 2])) Grid[x + 2, z + 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x - 2, z + 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z + 2], Grid[x - 2, z + 2].GetComponent<Renderer>().material));
                if (IsInCurSelectedCells(Grid[x - 2, z + 2])) Grid[x - 2, z + 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
        }
        //Check Lower
        if (IsZExist(z - 2))
        {
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x + 2, z - 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z - 2], Grid[x + 2, z - 2].GetComponent<Renderer>().material));
                if (IsInCurSelectedCells(Grid[x + 2, z - 2])) Grid[x + 2, z - 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x - 2, z - 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z - 2], Grid[x - 2, z - 2].GetComponent<Renderer>().material));
                if (IsInCurSelectedCells(Grid[x - 2, z - 2])) Grid[x - 2, z - 2].GetComponent<Renderer>().material = onSelectChoiseMaterial;
                CanAttack = true;
            }
        }

        if (CanAttack) return true;
        else return false;
    }
    public bool SerchCellsForKingAttack(int x, int z)
    {
        bool CanAttackHere = false;
        CanAttack = false;

        StonesColor CurAllyColor = StonesColor.Empty, CurEnemyColor = StonesColor.Empty;
        if (WhoseTurn == StonesColor.White)
        {
            CurAllyColor = StonesColor.White;
            CurEnemyColor = StonesColor.Black;
        }
        else
        {
            CurAllyColor = StonesColor.Black;
            CurEnemyColor = StonesColor.White;
        }

        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (!IsCellExist(leftX, tempZ)) break;
            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(leftX - 1, tempZ - 1) && !Grid[leftX - 1, tempZ - 1].GetComponent<CellScriptServer>().IsOcupied())
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
            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(rightX + 1, tempZ - 1) && !Grid[rightX + 1, tempZ - 1].GetComponent<CellScriptServer>().IsOcupied())
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
            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(leftX - 1, tempZ + 1) && !Grid[leftX - 1, tempZ + 1].GetComponent<CellScriptServer>().IsOcupied())
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
            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectChoiseMaterial;
            }

            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(rightX + 1, tempZ + 1) && !Grid[rightX + 1, tempZ + 1].GetComponent<CellScriptServer>().IsOcupied())
                {
                    CanAttackHere = true;
                    CanAttack = true;
                } else break;
            }
        }

        if (CanAttack) return true;
        else return false;
    }
    public bool SelectCellsToAttack(int x, int z)
    {
        IsSelectKing = false;
        CanAttack = false;
        IsAttackCombo = false;
        int MovesCount = CurSelectedCells.Count;

        StonesColor preferredColor = StonesColor.Empty;

        if (WhoseTurn == StonesColor.White) preferredColor = StonesColor.Black;
        else preferredColor = StonesColor.White;

        //Debug.Log(preferredColor);

        if (IsZExist(z + 2))
        {
            //Debug.Log("TRY RIGHT UP");
            if (IsXExist(x + 2) && Grid[x + 1, z + 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x + 2, z + 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z + 2], Grid[x + 2, z + 2].GetComponent<Renderer>().material));
                Grid[x + 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z + 2].GetComponent<CellScriptServer>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
                //Debug.Log("RIGHT UP");
            }
            //Debug.Log("TRY LEFT UP");
            if (IsXExist(x - 2) && Grid[x - 1, z + 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x - 2, z + 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z + 2], Grid[x - 2, z + 2].GetComponent<Renderer>().material));
                Grid[x - 2, z + 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x - 2, z + 2].GetComponent<CellScriptServer>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
                //Debug.Log("LEFT UP");
            }
        }
        //Check Lower
        if (IsZExist(z - 2))
        {
            //Debug.Log("TRY RIGHT DOWN");
            if (IsXExist(x + 2) && Grid[x + 1, z - 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x + 2, z - 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x + 2, z - 2], Grid[x + 2, z - 2].GetComponent<Renderer>().material));
                Grid[x + 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x + 2, z - 2].GetComponent<CellScriptServer>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
                //Debug.Log("RIGHT DOWN");
            }
            //Debug.Log("TRY LEFT DOWN");
            if (IsXExist(x - 2) && Grid[x - 1, z - 1].GetComponent<CellScriptServer>().WhatIsColor() == preferredColor && !Grid[x - 2, z - 2].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[x - 2, z - 2], Grid[x - 2, z - 2].GetComponent<Renderer>().material));
                Grid[x - 2, z - 2].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[x - 2, z - 2].GetComponent<CellScriptServer>().SetSelection(true);
                IsAttackCombo = true;
                CanAttack = true;
                //Debug.Log("LEFT DOWN");
            }
        }

        if (CanAttack)
        {
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

        StonesColor CurAllyColor = StonesColor.Empty, CurEnemyColor = StonesColor.Empty;
        if (WhoseTurn == StonesColor.White)
        {
            CurAllyColor = StonesColor.White;
            CurEnemyColor = StonesColor.Black;
        }
        else
        {
            CurAllyColor = StonesColor.Black;
            CurEnemyColor = StonesColor.White;
        }

        CanAttackHere = false;
        for (int leftX = x - 1, tempZ = z - 1; tempZ >= 0; leftX--, tempZ--)
        {
            if (!IsCellExist(leftX, tempZ)) break;
            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            //Debug.Log("KING LEFT DOWN");
            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }

            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(leftX - 1, tempZ - 1) && !Grid[leftX - 1, tempZ - 1].GetComponent<CellScriptServer>().IsOcupied())
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
            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;
            
            //Debug.Log("KING RIGHT DOWN");
            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }

            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(rightX + 1, tempZ - 1) && !Grid[rightX + 1, tempZ - 1].GetComponent<CellScriptServer>().IsOcupied())
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
            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            //Debug.Log("KING LEFT UP");
            if (CanAttackHere && !Grid[leftX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[leftX, tempZ], Grid[leftX, tempZ].GetComponent<Renderer>().material));
                Grid[leftX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[leftX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }

            if (Grid[leftX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(leftX - 1, tempZ + 1) && !Grid[leftX - 1, tempZ + 1].GetComponent<CellScriptServer>().IsOcupied())
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
            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurAllyColor) break;

            //Debug.Log("KING LEFT DOWN");
            if (CanAttackHere && !Grid[rightX, tempZ].GetComponent<CellScriptServer>().IsOcupied())
            {
                CurSelectedCells.Add(new SelectedItems(Grid[rightX, tempZ], Grid[rightX, tempZ].GetComponent<Renderer>().material));
                Grid[rightX, tempZ].GetComponent<Renderer>().material = onSelectMaterial;
                Grid[rightX, tempZ].GetComponent<CellScriptServer>().SetSelection(true);
            }

            if (Grid[rightX, tempZ].GetComponent<CellScriptServer>().WhatIsColor() == CurEnemyColor)
            { 
                if (IsCellExist(rightX + 1, tempZ + 1) && !Grid[rightX + 1, tempZ + 1].GetComponent<CellScriptServer>().IsOcupied())
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
            return true;
        } else return false;
    }
    public IEnumerator MoveSelectedStone(int endX, int endZ)
    {
        Debug.Log("MOVE STONE");
        Vector2Int startV = SH.GetSelectedStone();

        Debug.Log("Call move");
        SH.MoveStone(endX, endZ);

        ClearSelection();
        CanSelectStone.Clear();
        
        yield return new WaitForSeconds(MoveSpeed);

        DestroyEnemyStonesOnTheWay(startV.x, startV.y, endX, endZ);
        
        SetOcupied(endX, endZ, WhoseTurn);

        if (IsAttackCombo)
        {
            //Debug.Log(WhoseTurn);
            //Debug.Log("TryFind");
            Vector2Int temp = SH.GetSelectedStone();
            if (IsSelectKing)
            {
                //Debug.Log("Select Cell TO KING ATTACK");
                SelectCellsToKingAttack(temp.x, temp.y);
            }
            else
            {
                //Debug.Log("Select Cell TO ATTCK");
                SelectCellsToAttack(temp.x, temp.y);
            }
        }

        if (!IsAttackCombo)
        {
            Debug.Log("NextTurn");
            if (WhoseTurn == StonesColor.White) WhoseTurn = StonesColor.Black;
            else WhoseTurn = StonesColor.White;
        }

        selectedStone = null;

        SetCanSelect(false);
        ClearSelection();
        SH.FindTarget(WhoseTurn);
    }
    private void DestroyEnemyStonesOnTheWay(int startX, int startZ, int endX, int endZ)
    {
        if (startX < endX && startZ < endZ)
        {
            for (; startX < endX; startX++, startZ++)
            {
                if (Grid[startX, startZ].GetComponent<CellScriptServer>().IsOcupied())
                {
                    SH.FindAndDelete(startX, startZ);
                }
            }
            return;
        }
        if (startX > endX && startZ < endZ)
        {
            for (; startX > endX; startX--, startZ++)
            {
                if (Grid[startX, startZ].GetComponent<CellScriptServer>().IsOcupied())
                {
                    SH.FindAndDelete(startX, startZ);
                }
            }
            return;
        }
        if (startX < endX && startZ > endZ)
        {
            for (; startX < endX; startX++, startZ--)
            {
                if (Grid[startX, startZ].GetComponent<CellScriptServer>().IsOcupied())
                {
                    SH.FindAndDelete(startX, startZ);
                }
            }
            return;
        }
        if (startX > endX && startZ > endZ)
        {
            for (; startX > endX; startX--, startZ--)
            {
                if (Grid[startX, startZ].GetComponent<CellScriptServer>().IsOcupied())
                {
                    SH.FindAndDelete(startX, startZ);
                }
            }
            return;
        }
    }
    //Selection
    public void ClearSelection()
    {
        if (CurSelectedCells.Count > 0)
        {
            for (int i = 0; i < CurSelectedCells.Count; i++)
            {
                if (CurSelectedCells[i].obj) CurSelectedCells[i].obj.GetComponent<Renderer>().material = CurSelectedCells[i].objMaterial;
                if (CurSelectedCells[i].obj != selectedStone) CurSelectedCells[i].obj.GetComponent<CellScriptServer>().SetSelection(false);
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
                if (CurSelectedCells[i].obj != selectedStone) CurSelectedCells[i].obj.GetComponent<CellScriptServer>().SetSelection(false);
            }
        }
        CurSelectedCells.RemoveRange(start, end);
    }
    public bool CanSelect()
    {
        return CanSelectAnother;
    }
    public void SetCanSelect(bool b)
    {
        CanSelectAnother = b;
    }
    public void SetIsSelectKing(bool b)
    {
        IsSelectKing = b;
    }
    public bool IsInCanSelectList(GameObject obj)
    {
        if (CanSelectStone.Count == 0) return true;
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
    public StonesColor GetWhoseTurn()
    {
        return WhoseTurn;
    }
    public bool IsInCurSelectedCells(GameObject obj)
    {
        if (CurSelectedCells.Count == 0) return false;
        for (int i = 0; i < CurSelectedCells.Count; i++)
        {
            if (CurSelectedCells[i].obj == obj) return true;
        }
        return false;
    }
    //Cells
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
    public void SetOcupied(int x, int z, StonesColor color)
    {
        Grid[x, z].GetComponent<CellScriptServer>().SetOcupied(true, color);
    }
    public void SetUnOcupied(int x, int z)
    {
        Grid[x, z].GetComponent<CellScriptServer>().SetOcupied(false, StonesColor.Empty);
    }
}
