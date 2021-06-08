using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject EnemyStonesObj;
    private BoardScript BS;
    private List<GameObject> enemyStones = new List<GameObject>();
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
        if (enemyStones.Count == 0) return;
        bool isMoveMake = false;

        for (int i = 0; i < enemyStones.Count; i++)
        {
            if (BS.CheckEnemyAttack(enemyStones[i]) == true)
            {
                while (BS.CheckEnemyAttack(enemyStones[i]))
                {
                    Debug.Log("EnemyCombo");
                }
                isMoveMake = true;
                break;
            }
        }

        if (!isMoveMake)
        {
            for (int i = 0; i < 100; i++)
            {
                int MoveID = Random.Range(1, 3);
                int StonesID = Random.Range(0, enemyStones.Count);
                bool isFind = BS.EnemyMove((int)enemyStones[StonesID].transform.position.x, (int)enemyStones[StonesID].transform.position.z, enemyStones[StonesID], MoveID);
                if (isFind) break;
            }
        }
        BS.ClearSelection();
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

                if (enemyStones.Count == 0)
                {
                    BS.ShowCanvas();
                    //this.gameObject.SetActive(false);
                }

                return;
            }
        }
    }
}
