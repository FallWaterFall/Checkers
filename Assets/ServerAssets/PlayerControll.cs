using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerControll : MonoBehaviour
{
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                this.gameObject.transform.position += new Vector3(0, 0, 1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                this.gameObject.transform.position += new Vector3(0, 0, -1);
            }
        }
    }
}
