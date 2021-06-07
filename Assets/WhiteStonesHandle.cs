using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteStonesHandle : MonoBehaviour
{
    private BoardScript BS;
    private List<GameObject> whiteStones = new List<GameObject>();
    private void Start()
    {
        GameObject obj = GameObject.Find("Board");
        BS = obj.GetComponent<BoardScript>();
        for (int i = 0; i < 12; i++)
        {
            whiteStones.Add(this.transform.GetChild(i).gameObject);
        }
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

}
