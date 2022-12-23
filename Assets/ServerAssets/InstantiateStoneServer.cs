using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateStoneServer : MonoBehaviour
{
    [SerializeField] private GameObject allyStoneObj;
    [SerializeField] private GameObject enemyStoneObj;
    [SerializeField] private int boardSize;
    private StonesHandle SH;
}
