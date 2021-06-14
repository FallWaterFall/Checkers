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
    private GameObject moveAnimObj = null;
    private float moveAnimDeltaX, moveAnimDeltaZ, moveAnimEndX, moveAnimEndZ;
    private int moveAnimDirection;
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
    private void Update()
    {
        if (moveAnimObj != null) MoveStoneAnim();
    }
    public IEnumerator OpponentMove()
    {
        bool isMoveMake = false;
        int moveAmount = 0;

        if (enemyKingStones.Count > 0)
        {
            for (int i = 0; i < enemyKingStones.Count; i++)
            {
                if (BS.CheckEnemyKingAttack(enemyKingStones[i]))
                {
                    moveAmount++;
                    isMoveMake = true;
                }
            }
        }

        if (!isMoveMake && enemyStones.Count > 0)
        {
            for (int i = 0; i < enemyStones.Count; i++)
            {
                if (BS.CheckEnemyAttack(enemyStones[i]) == true)
                {
                    moveAmount++;
                    yield return new WaitForSeconds(0.51f);
                    while (BS.CheckEnemyAttack(enemyStones[i]) == true)
                    {
                        moveAmount++;
                        yield return new WaitForSeconds(0.51f);
                        Debug.Log("EnemyCombo");
                    }
                    isMoveMake = true;
                    break;
                }
            }
        }

        if (!isMoveMake && enemyStones.Count > 0)
        {
            for (int i = 0; i < 200; i++)
            {
                int MoveID = Random.Range(1, 3);
                int StonesID = Random.Range(0, enemyStones.Count);
                moveAnimObj = enemyStones[StonesID];

                if (BS.EnemyMove((int)enemyStones[StonesID].transform.position.x, (int)enemyStones[StonesID].transform.position.z, MoveID))
                {
                    moveAmount++;
                    isMoveMake = true;
                    break;
                }
            }
        }

        if (!isMoveMake && enemyKingStones.Count > 0)
        {
            for (int i = 0; i < 20; i++)
            {
                int StonesID = Random.Range(0, enemyKingStones.Count);

                if (BS.MoveEnemyKing((int)enemyKingStones[StonesID].transform.position.x, (int)enemyKingStones[StonesID].transform.position.z, enemyKingStones[StonesID]))
                {
                    moveAmount++;
                    isMoveMake = true;
                    break;
                }
            }
        }

        if (!isMoveMake)
        {
            Debug.Log("Enemy stones can't move");
            BS.ShowCanvas(true);
            BS.SetCanSelect(false);
        }
        else
        {
            yield return new WaitForSeconds(moveAmount * 0.51f);
            BS.SetCanSelect(true);
            BS.FindTarget();
        }
    }
    public void MakeMoveAnim(int endX, int endZ)
    {
        BS.SetCanSelect(false);

        moveAnimEndX = endX;
        moveAnimEndZ = endZ;
        moveAnimDeltaX = (endX - moveAnimObj.transform.position.x) / 25f;
        moveAnimDeltaZ = (endZ - moveAnimObj.transform.position.z) / 25f;
        if (moveAnimObj.transform.position.x < endX) moveAnimDirection = 0;
        else moveAnimDirection = 1;
    }
    public void MakeMoveAnim(int endX, int endZ, GameObject obj)
    {
        BS.SetCanSelect(false);

        moveAnimObj = obj;
        moveAnimEndX = endX;
        moveAnimEndZ = endZ;
        moveAnimDeltaX = (endX - moveAnimObj.transform.position.x) / 25f;
        moveAnimDeltaZ = (endZ - moveAnimObj.transform.position.z) / 25f;
        if (moveAnimObj.transform.position.x < endX) moveAnimDirection = 0;
        else moveAnimDirection = 1;
    }
    private void MoveStoneAnim()
    {
        moveAnimObj.transform.position += new Vector3(moveAnimDeltaX, 0, moveAnimDeltaZ);

        if ((moveAnimObj.transform.position.x >= moveAnimEndX && moveAnimDirection == 0) || (moveAnimObj.transform.position.x <= moveAnimEndX && moveAnimDirection == 1))
        {
            moveAnimObj.transform.position = new Vector3(moveAnimEndX, 0.2f , moveAnimEndZ);

            moveAnimDeltaX = 0;
            moveAnimDeltaZ = 0;
            moveAnimEndX = 0;
            moveAnimEndZ = 0;
            if (moveAnimObj.transform.position.z == 0 && FindIndexOfObj(moveAnimObj) != -1) ChangeStoneToKing(FindIndexOfObj(moveAnimObj));
            moveAnimObj = null;
        }
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
            Debug.Log("Enemy stones < 0");
            BS.ShowCanvas(true);
            BS.SetCanSelect(false);
        }
    }
    public void ChangeStoneToKing(int i)
    {
        Debug.Log("Change to enemy king");
        var obj = enemyStones[i];
        enemyStones.Remove(obj);
        var newObj = Instantiate(enemyKingObj, obj.transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(obj);
        newObj.transform.SetParent(EnemyStonesObj.transform);
        enemyKingStones.Add(newObj);
    }
    private int FindIndexOfObj(GameObject obj)
    {
        for (int i = 0; i < enemyStones.Count; i++)
        {
            if (obj == enemyStones[i]) return i;
        }
        return -1;
    }
}
