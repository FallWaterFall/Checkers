using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
    private BoardScript BS;
    [SerializeField] private Material onSelectMaterial;
    private Material commonMaterial;
    private void Start()
    {
        commonMaterial = this.GetComponent<Renderer>().material;
        GameObject obj = GameObject.Find("Board");
        BS = obj.GetComponent<BoardScript>();
        BS.SetOcupied((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.z, Color.White);
    }
    private void OnMouseDown()
    {
        if (!BS.IsCombo())
            BS.OnSelect((int)this.transform.position.x, (int)this.transform.position.z, this.gameObject);
    }
}
