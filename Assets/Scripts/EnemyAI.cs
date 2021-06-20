using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject boardObj;
    [SerializeField] GameObject enemyKingObj;
    [SerializeField] private List<GameObject> enemyStones = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyKingStones = new List<GameObject>();
    private BoardScript BS;
    private GameObject moveAnimObj = null;
    private float moveAnimDeltaX, moveAnimDeltaZ, moveAnimEndX, moveAnimEndZ;
    private int moveAnimDirection;
    private bool IsChangedOnKing = false;
    private void Start()
    {
        BS = boardObj.GetComponent<BoardScript>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            enemyStones.Add(this.transform.GetChild(i).gameObject);
            BS.SetOcupied((int)enemyStones[i].transform.position.x, (int)enemyStones[i].transform.position.z, Color.Black);
        }
    }
    private void FixedUpdate()
    {
        if (moveAnimObj != null) MoveStoneAnim();
    }
    public IEnumerator OpponentMove()
    {
        bool isMoveMake = false;
        IsChangedOnKing = false;
        int moveAmount = 0;

        //Chek if Enemy King can attack
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
        //Chek if Enemy Stone can attack
        if (!isMoveMake && enemyStones.Count > 0)
        {
            for (int i = 0; i < enemyStones.Count; i++)
            {
                if (BS.CheckEnemyAttack(enemyStones[i]) == true)
                {
                    yield return new WaitForSeconds(BS.MoveSpeed);
                    while (BS.CheckEnemyAttack(enemyStones[i]) == true)
                    {
                        yield return new WaitForSeconds(BS.MoveSpeed);
                        if (IsChangedOnKing) break;
                        Debug.Log("EnemyCombo");
                    }
                    isMoveMake = true;
                    break;
                }
            }
        }
        //Chek if Enemy Stone can move
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
        //Chek if Enemy King can move
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

        if (IsChangedOnKing)
        {
            StartCoroutine(OpponentMove());
        }
        else if (!isMoveMake && enemyKingStones.Count + enemyStones.Count > 0)
        {
            Debug.Log("Enemy stones can't move");
            BS.EndGame(true);
        }
        else
        {
            yield return new WaitForSeconds(moveAmount * BS.MoveSpeed);
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
            BS.EndGame(true);
        }
    }
    public void ChangeStoneToKing(int i)
    {
        Debug.Log("Change to enemy king");
        var obj = enemyStones[i];
        enemyStones.Remove(obj);
        var newObj = Instantiate(enemyKingObj, obj.transform.position, Quaternion.identity);
        Destroy(obj);
        newObj.transform.SetParent(this.transform);
        enemyKingStones.Add(newObj);
        IsChangedOnKing = true;
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
