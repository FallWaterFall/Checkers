using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateStones : MonoBehaviour
{
    [SerializeField] private Transform allyHadle;
    [SerializeField] private Transform enemyHandle;
    [SerializeField] private List<GameObject> allyStoneObj;
    [SerializeField] private List<GameObject> enemyStoneObj;
    [SerializeField] private List<Color32> AllyStoneColor;
    [SerializeField] private int boardSize;
    private void Awake()
    {
        int ModelNum = DataBetweenScenes.Model;
        int ColorNum = DataBetweenScenes.Color;

        for (int i = 0; i < boardSize / 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var allyObj = Instantiate(allyStoneObj[ModelNum], new Vector3(i * 2 + j % 2, 0.2f, j), allyStoneObj[ModelNum].transform.rotation);
                
                Material[] matArray = new Material[2];
                matArray[0] = allyObj.GetComponent<Renderer>().material;
                matArray[1] = matArray[0];
                matArray[1].color = AllyStoneColor[ColorNum];
                allyObj.GetComponent<Renderer>().materials = matArray;

                allyObj.transform.SetParent(allyHadle);
                var enemyObj = Instantiate(enemyStoneObj[ModelNum], new Vector3(i * 2 + (j + 1) % 2, 0.2f, boardSize - j - 1), enemyStoneObj[ModelNum].transform.rotation);
                enemyObj.transform.SetParent(enemyHandle);
            }
        }
    }
}
