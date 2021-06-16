using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateStones : MonoBehaviour
{
    [SerializeField] private Transform allyHadle;
    [SerializeField] private Transform enemyHandle;
    [SerializeField] private List<GameObject> allyStoneObj;
    [SerializeField] private List<GameObject> enemyStoneObj;
    [SerializeField] private int boardSize;
    private void Awake()
    {
        int SkinNum = DataBetweenScenes.Skin;

        for (int i = 0; i < boardSize / 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var allyObj = Instantiate(allyStoneObj[SkinNum], new Vector3(i * 2 + j % 2, 0.2f, j), allyStoneObj[SkinNum].transform.rotation);
                allyObj.transform.SetParent(allyHadle);
                var enemyObj = Instantiate(enemyStoneObj[SkinNum], new Vector3(i * 2 + (j + 1) % 2, 0.2f, boardSize - j - 1), enemyStoneObj[SkinNum].transform.rotation);
                enemyObj.transform.SetParent(enemyHandle);
            }
        }
    }
}
