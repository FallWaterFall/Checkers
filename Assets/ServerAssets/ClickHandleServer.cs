using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandleServer : MonoBehaviour
{
    private StonesHandle SH;
    private BoardServer BS;
    [SerializeField] private GameObject StonesObj;
    [SerializeField] private GameObject boardObj;
    private void Start()
    {
        SH = GameObject.Find("Pivot").GetComponent<StonesHandle>();
        BS = GameObject.Find("Board").GetComponent<BoardServer>();
    }
    private void Update()
    {
        if (Input.anyKey)
        {
            bool castRay = false;
            Ray raycast = new Ray();
            if (Application.platform == RuntimePlatform.Android && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                castRay = true;
            }
            else if ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer) && Input.GetMouseButtonDown(0))
            {
                raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
                castRay = true;
            }
            if (castRay)
            {
                RaycastHit raycastHit;

                if (Physics.Raycast(raycast, out raycastHit))
                {
                    int x = (int)raycastHit.transform.localPosition.x;
                    int z = (int)raycastHit.transform.localPosition.z;

                    if (raycastHit.transform.gameObject.tag == "AllyStone")
                    {
                        SH.SelectStone(x, z, StonesColor.White);
                    }
                    if (raycastHit.transform.gameObject.tag == "KingAllyStone")
                    {
                        SH.SelectKingStone(x, z, raycastHit.transform.gameObject, StonesColor.White);
                    }
                    if (raycastHit.transform.gameObject.tag == "EnemyStone")
                    {
                        SH.SelectStone(x, z, StonesColor.Black);
                    }
                    if (raycastHit.transform.gameObject.tag == "KingEnemyStone")
                    {
                        SH.SelectKingStone(x, z, raycastHit.transform.gameObject, StonesColor.Black);
                    }
                    if (raycastHit.transform.gameObject.tag == "Cell")
                    {
                        if (BS.GetCell(x, z).GetComponent<CellScriptServer>().IsSelected())
                        {
                            StartCoroutine(BS.MoveSelectedStone((int)raycastHit.transform.localPosition.x, (int)raycastHit.transform.localPosition.z));
                        }
                    }
                }
            }
        }
    }
}
