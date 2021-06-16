using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteStonesHandle : MonoBehaviour
{
    [SerializeField] private Material onSelectMaterial;
    [SerializeField] private GameObject KingObj;
    private BoardScript BS;
    private List<GameObject> whiteStones = new List<GameObject>();
    private SelectedItems SelectedStone;
    private GameObject moveAnimObj = null;
    private float moveAnimDeltaX, moveAnimDeltaZ, moveAnimEndX, moveAnimEndZ;
    private int moveAnimDirection;
    private void Start()
    {
        GameObject obj = GameObject.Find("Board");
        BS = obj.GetComponent<BoardScript>();
        for (int i = 0; i < 12; i++)
        {
            obj = this.transform.GetChild(i).gameObject;
            BS.SetOcupied((int)obj.transform.position.x, (int)obj.transform.position.z, Color.White);
            
            whiteStones.Add(obj);
        }
    }
    public void Update()
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
        moveAnimDeltaX = (endX - SelectedStone.obj.transform.position.x) / 50.0f;
        moveAnimDeltaZ = (endZ - SelectedStone.obj.transform.position.z) / 50.0f;

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
            if (moveAnimObj.transform.position.z == 7) ChangeStoneOnKing();
            moveAnimObj = null;
            
            BS.SetCanSelect(false);
        }
    }
    private void ChangeStoneOnKing()
    {
        int i;
        var king = Instantiate(KingObj, SelectedStone.obj.transform.position, Quaternion.Euler(-90, 0, 0));
        king.transform.SetParent(this.transform);

        for (i = 0; i < whiteStones.Count; i++)
            if (whiteStones[i] == SelectedStone.obj)
                break;
        
        Destroy(whiteStones[i]);
        whiteStones[i] = king;
        SelectedStone.obj = king;
        moveAnimObj = king;
    }
    public void FindTarget()
    {
        for (int i = 0; i < whiteStones.Count; i++)
        {
            SelectStone(whiteStones[i]);
            if (whiteStones[i].GetComponent<StoneScript>() != null)
                if (BS.SerchCellsForAttack((int)whiteStones[i].transform.position.x, (int)whiteStones[i].transform.position.z)) BS.AddInCanSelectList(whiteStones[i]);
            if (whiteStones[i].GetComponent<KingStoneScript>() != null)
                if (BS.SerchCellsForKingAttack((int)whiteStones[i].transform.position.x, (int)whiteStones[i].transform.position.z)) BS.AddInCanSelectList(whiteStones[i]);
        }
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;
        
        SelectedStone.obj = null;
    }
    public void FindAndDelete(int x, int z)
    {
        for (int i = 0; i < whiteStones.Count; i++)
        {
            if ((int)whiteStones[i].transform.position.x == x && (int)whiteStones[i].transform.position.z == z)
            {
                GameObject obj = whiteStones[i];
                whiteStones.Remove(obj);
                BS.SetUnOcupied((int)obj.transform.position.x, (int)obj.transform.position.z);
                Destroy(obj);
            }
        }
        if (whiteStones.Count == 0) BS.EndGame(false); 
    }
    public Vector2Int GetSelectedStone()
    {
        return new Vector2Int((int)SelectedStone.obj.transform.position.x, (int)SelectedStone.obj.transform.position.z);
    }
}
