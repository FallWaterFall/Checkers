using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneComponentRotator : MonoBehaviour
{
    private float x = 0.1f, y = 0.1f, z = 0.1f;
    private void Start()
    {
        int temp;
        temp = Random.Range(0, 2);
        if (temp == 0) x *= -1;
        temp = Random.Range(0, 2);
        if (temp == 0) y *= -1;
        temp = Random.Range(0, 2);
        if (temp == 0) z *= -1;
    }
    private void FixedUpdate()
    {
        this.transform.Rotate(new Vector3(x, y, z));
    }
}
