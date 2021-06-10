using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject EnemyStonesObj;
    [SerializeField] GameObject enemyKingObj;
    private BoardScript BS;
    private List<GameObject> enemyStones = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyKingStones = new List<GameObject>();
    private void Start()
    {
        GameObject obj = GameObject.Find("Board");
        BS = obj.GetComponent<BoardScript>();
        for (int i = 0; i < 12; i++)
        {
            enemyStones.Add(EnemyStonesObj.transform.GetChild(i).gameObject);
            BS.SetOcupied((int)enemyStones[i].transform.position.x, (int)enemyStones[i].transform.position.z, Color.Black);
        }
    }
    public void OpponentMove()
    {
        if (enemyStones.Count + enemyKingStones.Count == 0) return;
        bool isMoveMake = false;

        if (enemyKingStones.Count > 0)
        {
            for (int i = 0; i < enemyKingStones.Count; i++)
            {
                if (BS.CheckEnemyKingAttack(enemyKingStones[i]) == true)
                {
                    isMoveMake = true;
                    Debug.Log("CheckEnemyKingAttack ");
                }
            }
        }

        if (!isMoveMake)
        {
            for (int i = 0; i < enemyStones.Count; i++)
            {
                if (BS.CheckEnemyAttack(enemyStones[i]) == true)
                {
                    while (BS.CheckEnemyAttack(enemyStones[i]))
                    {
                        Debug.Log("EnemyCombo");
                    }

                    if (enemyStones[i].transform.position.z == 0) ChangeStoneToKing(i);
                    isMoveMake = true;
                    break;
                }
            }
        }

        if (!isMoveMake && enemyStones.Count > 0)
        {
            for (int i = 0; i < 100; i++)
            {
                int MoveID = Random.Range(1, 3);
                int StonesID = Random.Range(0, enemyStones.Count);
                bool isFind = BS.EnemyMove((int)enemyStones[StonesID].transform.position.x, (int)enemyStones[StonesID].transform.position.z, enemyStones[StonesID], MoveID);
                if (isFind)
                {
                    if (enemyStones[StonesID].transform.position.z == 0) ChangeStoneToKing(StonesID);
                    isMoveMake = true;
                    break;
                }
            }
        }

        if (!isMoveMake)
        {
            for (int i = 0; i < 20; i++)
            {
                int StonesID = Random.Range(0, enemyKingStones.Count);
                Debug.Log("Enemy king must move");
                bool isFind = BS.MoveEnemyKing((int)enemyKingStones[StonesID].transform.position.x, (int)enemyKingStones[StonesID].transform.position.z, enemyKingStones[StonesID]);
                if (isFind) break;
            }
        }

        BS.ClearSelection();

        if (!isMoveMake) BS.ShowCanvas();
    }
    public void KillStones(int x, int z)
    {
        for (int i = 0; i < enemyStones.Count; i++)
        {
            if ((int)enemyStones[i].transform.position.x == x && (int)enemyStones[i].transform.position.z == z)
            {
                GameObject obj = enemyStones[i];
                enemyStones.Remove(obj);
                Destroy(obj);
                BS.SetUnOcupied(x, z);
            }
        }
        for (int i = 0; i < enemyKingStones.Count; i++)
        {
            if ((int)enemyKingStones[i].transform.position.x == x && (int)enemyKingStones[i].transform.position.z == z)
            {
                GameObject obj = enemyKingStones[i];
                enemyKingStones.Remove(obj);
                Destroy(obj);
                BS.SetUnOcupied(x, z);
            }
        }
        if (enemyStones.Count + enemyKingStones.Count == 0)
        {
            BS.ShowCanvas();
            //this.gameObject.SetActive(false);
        }
    }
    public void ChangeStoneToKing(int i)
    {
        var obj = enemyStones[i];
        enemyStones.Remove(obj);
        var newObj = Instantiate(enemyKingObj, obj.transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(obj);
        newObj.transform.SetParent(EnemyStonesObj.transform);
        enemyKingStones.Add(newObj);
    }
}
