using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingStoneScript : MonoBehaviour
{
    private WhiteStonesHandle WSH;
    private Material commonMaterial;
    private void Start()
    {
        commonMaterial = this.GetComponent<Renderer>().material;
        WSH = this.transform.GetComponentInParent<WhiteStonesHandle>();
    }
    private void OnMouseDown()
    {
        WSH.SelectKingStone((int)this.transform.position.x, (int)this.transform.position.z, this.gameObject);
    }
}
