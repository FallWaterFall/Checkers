using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    private float latestMousePosX;
    private float latestMousePosY;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            latestMousePosX = Input.mousePosition.x;
            latestMousePosY = Input.mousePosition.y;
        }
        if (Input.GetMouseButton(1))
        {
            float deltaX = Input.mousePosition.x - latestMousePosX;
            float deltaY = Input.mousePosition.y - latestMousePosY;
            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                this.transform.GetChild(0).transform.Rotate(0, 0, speed * deltaX);
            }
            else
            {
                transform.Rotate(speed * deltaY, 0, 0);
            }
            latestMousePosX = Input.mousePosition.x;
            latestMousePosY = Input.mousePosition.y;
        }
    }
}
