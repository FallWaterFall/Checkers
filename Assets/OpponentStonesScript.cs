using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentStonesScript : MonoBehaviour
{
    private BoardScript BS;
    private void Start()
    {
        GameObject obj = GameObject.Find("Board");
        BS = obj.GetComponent<BoardScript>();
        BS.SetOcupied((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.z, Color.Black);
    }
}
