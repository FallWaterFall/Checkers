using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyStonesHandle : MonoBehaviour
{
    [SerializeField] private Material onSelectMaterial;
    [SerializeField] private GameObject KingObj;
    [SerializeField] private GameObject boardObj;
    private BoardScript BS;
    private List<GameObject> allyStones = new List<GameObject>();
    private SelectedItems SelectedStone;
    private GameObject moveAnimObj = null;
    private float moveAnimDeltaX, moveAnimDeltaZ, moveAnimEndX, moveAnimEndZ;
    private int moveAnimDirection;
    private void Start()
    {
        BS = boardObj.GetComponent<BoardScript>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            boardObj = this.transform.GetChild(i).gameObject;
            BS.SetOcupied((int)boardObj.transform.position.x, (int)boardObj.transform.position.z, StonesColor.White);
            
            allyStones.Add(boardObj);
        }
    }
    public void FixedUpdate()
    {
        if (moveAnimObj != null) MoveStoneAnim();
    }
    public void SelectStone(GameObject obj)
    {
        if (!BS.CanSelect() && !BS.IsInCanSelectList(obj)) return;
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;
    }
    public void SelectStone(int x, int z, GameObject obj)
    {
        if (!BS.CanSelect() && !BS.IsInCanSelectList(obj)) return;

        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;

        BS.SelectCells(x, z);
    }
    public void SelectKingStone(int x, int z, GameObject obj)
    {
        if (!BS.CanSelect() && !BS.IsInCanSelectList(obj)) return;
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;
        if (!BS.SelectCellsToKingAttack(x, z)) BS.SelectCellsForKing(x, z);
    }
    public void MoveStone(int endX, int endZ)
    {
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
            moveAnimObj.transform.position = new Vector3(moveAnimEndX, moveAnimObj.transform.position.y - 0.2f , moveAnimEndZ);

            moveAnimDeltaX = 0;
            moveAnimDeltaZ = 0;
            moveAnimEndX = 0;
            moveAnimEndZ = 0;
            if (moveAnimObj.transform.position.z == 7 && moveAnimObj.tag != "KingAllyStone") ChangeStoneOnKing();
            moveAnimObj = null;
            
            BS.SetCanSelect(false);
        }
    }
    private void ChangeStoneOnKing()
    {
        int i;
        var king = Instantiate(KingObj, SelectedStone.obj.transform.position, Quaternion.identity);
        king.transform.SetParent(this.transform);

        for (i = 0; i < allyStones.Count; i++)
            if (allyStones[i] == SelectedStone.obj)
                break;
        
        Destroy(allyStones[i]);
        allyStones[i] = king;
        SelectedStone.obj = king;
        moveAnimObj = king;
        BS.SetIsSelectKing(true);
    }
    public void FindTarget()
    {
        for (int i = 0; i < allyStones.Count; i++)
        {
            //SelectStone(whiteStones[i]);
            if (allyStones[i].gameObject.tag == "AllyStone")
                if (BS.SerchCellsForAttack((int)allyStones[i].transform.position.x, (int)allyStones[i].transform.position.z)) BS.AddInCanSelectList(allyStones[i]);
            if (allyStones[i].gameObject.tag == "KingAllyStone")
                if (BS.SerchCellsForKingAttack((int)allyStones[i].transform.position.x, (int)allyStones[i].transform.position.z)) BS.AddInCanSelectList(allyStones[i]);
        }
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;
        SelectedStone.obj = null;
    }
    public IEnumerator FindAndDelete(int x, int z)
    {
        for (int i = 0; i < allyStones.Count; i++)
        {
            if ((int)allyStones[i].transform.position.x == x && (int)allyStones[i].transform.position.z == z)
            {
                yield return new WaitForSeconds(0.5f);
                GameObject obj = allyStones[i];
                allyStones.Remove(obj);
                BS.SetUnOcupied((int)obj.transform.position.x, (int)obj.transform.position.z);
                Destroy(obj);
            }
        }
        if (allyStones.Count == 0) BS.EndGame(false); 
    }
    public Vector2Int GetSelectedStone()
    {
        return new Vector2Int((int)SelectedStone.obj.transform.position.x, (int)SelectedStone.obj.transform.position.z);
    }
}
