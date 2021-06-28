using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonesHandle : MonoBehaviour
{
    [SerializeField] private Material onSelectMaterial;
    [SerializeField] private GameObject AllyKingObj, EnemyKingObj;
    private BoardServer BS;
    [SerializeField] private List<GameObject> allyStones = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyStones = new List<GameObject>();
    private SelectedItems SelectedStone;
    private GameObject moveAnimObj = null;
    private float moveAnimDeltaX, moveAnimDeltaZ, moveAnimEndX, moveAnimEndZ;
    private int moveAnimDirection;
    private void Start()
    {
        BS = GameObject.Find("Board").GetComponent<BoardServer>();

        for (int i = 0; i < this.transform.GetChild(0).childCount; i++)
        {
            var stone = this.transform.GetChild(0).GetChild(i).gameObject;
            BS.SetOcupied((int)stone.transform.position.x, (int)stone.transform.position.z, StonesColor.White);
            
            allyStones.Add(stone);
        }
        for (int i = 0; i < this.transform.GetChild(1).childCount; i++)
        {
            var stone = this.transform.GetChild(1).GetChild(i).gameObject;
            BS.SetOcupied((int)stone.transform.position.x, (int)stone.transform.position.z, StonesColor.Black);
            
            enemyStones.Add(stone);
        }
    }
    public void FixedUpdate()
    {
        if (moveAnimObj != null) MoveStoneAnim();
    }
    public void SelectStone(GameObject obj)
    {
        if (!BS.IsInCanSelectList(obj)) return;
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;
    }
    public void SelectStone(int x, int z, StonesColor clickedColor)
    {
        var obj = FindStoneObj(x, z);

        if (BS.GetWhoseTurn() != clickedColor || (!BS.CanSelect() && !BS.IsInCanSelectList(obj))) return;

        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;

        BS.SelectCells(x, z);
    }
    public void SelectKingStone(int x, int z, GameObject obj, StonesColor clickedColor)
    {
        if (BS.GetWhoseTurn() != clickedColor || (!BS.CanSelect() && !BS.IsInCanSelectList(obj))) return;
        //if (!BS.IsInCanSelectList(obj)) return;
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;

        if (!BS.SelectCellsToKingAttack(x, z)) BS.SelectCellsForKing(x, z);
    }
    public void MoveStone(int endX, int endZ)
    {
        Debug.Log("MoveStone start");
        BS.SetUnOcupied((int)SelectedStone.obj.transform.position.x, (int)SelectedStone.obj.transform.position.z);

        SelectedStone.obj.transform.position += new Vector3(0, 0.2f, 0);
        moveAnimObj = SelectedStone.obj;
        moveAnimEndX = endX;
        moveAnimEndZ = endZ;
        moveAnimDeltaX = (endX - SelectedStone.obj.transform.position.x) / 25f;
        moveAnimDeltaZ = (endZ - SelectedStone.obj.transform.position.z) / 25f;

        if (SelectedStone.obj.transform.position.x < endX) moveAnimDirection = 0;
        else moveAnimDirection = 1;

        SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;
    }
    private void MoveStoneAnim()
    {
        moveAnimObj.transform.position += new Vector3(moveAnimDeltaX, 0, moveAnimDeltaZ);
        if ((moveAnimObj.transform.position.x >= moveAnimEndX && moveAnimDirection == 0) || (moveAnimObj.transform.position.x <= moveAnimEndX && moveAnimDirection == 1))
        {
            moveAnimObj.transform.position = new Vector3(moveAnimEndX, 0.2f , moveAnimEndZ);

            moveAnimDeltaX = 0;
            moveAnimDeltaZ = 0;
            moveAnimEndX = 0;
            moveAnimEndZ = 0;
            if ((moveAnimObj.transform.position.z == 7 && moveAnimObj.tag == "AllyStone") || (moveAnimObj.transform.position.z == 0 && moveAnimObj.tag == "EnemyStone")) ChangeStoneOnKing();
            moveAnimObj = null;
        }
    }
    private void ChangeStoneOnKing()
    {
        List<GameObject> prefList = null;
        GameObject prefKing = null;
        Debug.Log(BS.GetWhoseTurn());
        if (BS.GetWhoseTurn() == StonesColor.White)
        {
            prefList = allyStones;
            prefKing = AllyKingObj;
        }
        else
        {
            prefList = enemyStones;
            prefKing = EnemyKingObj;
        }

        int i = 0;
        Debug.Log(SelectedStone.obj.name);
        var king = Instantiate(prefKing, SelectedStone.obj.transform.position, Quaternion.identity);
        
        if (BS.GetWhoseTurn() == StonesColor.White) king.transform.SetParent(this.transform.GetChild(0));
        else king.transform.SetParent(this.transform.GetChild(1));
        Debug.Log(prefList.Count);
        for (i = 0; i < prefList.Count; i++)
        {
            Debug.Log(i);
            if (prefList[i] == SelectedStone.obj) break;
        }
        Debug.Log(prefList[i]);
        var toDestroy = prefList[i];
        prefList[i] = king;
        Destroy(toDestroy);
        SelectedStone.obj = king;
        BS.SetIsSelectKing(true);
    }
    public void FindTarget(StonesColor WhoseTurn)
    {
        if (WhoseTurn == StonesColor.White)
        {
            for (int i = 0; i < allyStones.Count; i++)
            {
                //SelectStone(allyStones[i]);
                if (allyStones[i].gameObject.tag == "AllyStone")
                    if (BS.SerchCellsForAttack((int)allyStones[i].transform.position.x, (int)allyStones[i].transform.position.z)) BS.AddInCanSelectList(allyStones[i]);
                if (allyStones[i].gameObject.tag == "KingAllyStone")
                    if (BS.SerchCellsForKingAttack((int)allyStones[i].transform.position.x, (int)allyStones[i].transform.position.z)) BS.AddInCanSelectList(allyStones[i]);
            }
        }
        else
        {
            for (int i = 0; i < enemyStones.Count; i++)
            {
                //SelectStone(enemyStones[i]);
                if (enemyStones[i].gameObject.tag == "EnemyStone")
                    if (BS.SerchCellsForAttack((int)enemyStones[i].transform.position.x, (int)enemyStones[i].transform.position.z)) BS.AddInCanSelectList(enemyStones[i]);
                if (enemyStones[i].gameObject.tag == "KingEnemyStone")
                    if (BS.SerchCellsForKingAttack((int)enemyStones[i].transform.position.x, (int)enemyStones[i].transform.position.z)) BS.AddInCanSelectList(enemyStones[i]);
            }
        }
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;
        //SelectedStone.obj = null;
    }
    public void FindAndDelete(int x, int z)
    {
        List<GameObject> prefColorList;
        if (BS.GetWhoseTurn() == StonesColor.White) prefColorList = enemyStones;
        else  prefColorList = allyStones;

        for (int i = 0; i < prefColorList.Count; i++)
        {
            if ((int)prefColorList[i].transform.position.x == x && (int)prefColorList[i].transform.position.z == z)
            {
                GameObject obj = prefColorList[i];
                prefColorList.Remove(obj);
                BS.SetUnOcupied((int)obj.transform.position.x, (int)obj.transform.position.z);
                Destroy(obj);
            }
        }

        if (allyStones.Count == 0) Debug.Log("END GAME. ENEMY WIN");
        if (enemyStones.Count == 0) Debug.Log("END GAME. ALLY WIN");
    }
    public Vector2Int GetSelectedStone()
    {
        return new Vector2Int((int)SelectedStone.obj.transform.position.x, (int)SelectedStone.obj.transform.position.z);
    }
    private GameObject FindStoneObj(int x, int z)
    {
        for (int i = 0; i < allyStones.Count; i++)
        {
            if ((int)allyStones[i].transform.position.x == x && (int)allyStones[i].transform.position.z == z) return allyStones[i];
        }
        for (int i = 0; i < enemyStones.Count; i++)
        {
            if ((int)enemyStones[i].transform.position.x == x && (int)enemyStones[i].transform.position.z == z) return enemyStones[i];
        }
        return null;
    }
}