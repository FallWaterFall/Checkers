using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandle : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CastRay();
        }
    }
    private void CastRay()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.Log("Click " + Camera.main.ScreenPointToRay(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform.gameObject.tag == "Stones")
            {
                Debug.Log("Selected " + hit.transform.gameObject.name);
            }
        }
    }
}
