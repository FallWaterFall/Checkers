using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InstantiateStoneServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform allyHadle;
    [SerializeField] private Transform enemyHandle;
    [SerializeField] private GameObject allyStoneObj;
    [SerializeField] private GameObject enemyStoneObj;
    [SerializeField] private int boardSize;
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < boardSize / 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var allyObj = PhotonNetwork.InstantiateRoomObject("AllyStoneServ1", new Vector3(i * 2 + j % 2, 0.2f, j), allyStoneObj.transform.rotation);
                    allyObj.transform.GetChild(0).SetParent(allyHadle);
                    var enemyObj = PhotonNetwork.InstantiateRoomObject("EnemyStoneServ1", new Vector3(i * 2 + (j + 1) % 2, 0.2f, boardSize - j - 1), enemyStoneObj.transform.rotation);
                    enemyObj.transform.GetChild(1).SetParent(enemyHandle);
                }
            }
        }
    }
}
