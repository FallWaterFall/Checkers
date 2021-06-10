using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
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
        WSH.SelectStone((int)this.transform.position.x, (int)this.transform.position.z, this.gameObject);
    }
}
