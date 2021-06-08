using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteStonesHandle : MonoBehaviour
{
    [SerializeField] private Material onSelectMaterial;
    private BoardScript BS;
    private List<GameObject> whiteStones = new List<GameObject>();
    private SelectedItems SelectedStone;
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
    public void SelectStone(GameObject obj)
    {
        if (!BS.CanSelect()) return;
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;
    }
    public void SelectStone(int x, int z, GameObject obj)
    {
        if (!BS.CanSelect()) return;
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

        Renderer objRend = obj.GetComponent<Renderer>();
        SelectedStone = new SelectedItems(obj, objRend.material);
        objRend.material = onSelectMaterial;

        BS.SelectCells(x, z);
    }
    public void MoveStone(int endX, int endZ)
    {
        BS.SetUnOcupied((int)SelectedStone.obj.transform.position.x, (int)SelectedStone.obj.transform.position.z);

        SelectedStone.obj.transform.position = new Vector3(endX, 0.2f, endZ);
        SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;

    }
    public void FindTarget()
    {
        for (int i = 0; i < whiteStones.Count; i++)
        {
            SelectStone(whiteStones[i]);
            if (BS.SelectCellsToAttack((int)whiteStones[i].transform.position.x, (int)whiteStones[i].transform.position.z)) return;
        }
        if (SelectedStone.obj != null) SelectedStone.obj.GetComponent<Renderer>().material = SelectedStone.objMaterial;
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
    }
    public Vector2Int GetSelectedStone()
    {
        return new Vector2Int((int)SelectedStone.obj.transform.position.x, (int)SelectedStone.obj.transform.position.z);
    }
}
