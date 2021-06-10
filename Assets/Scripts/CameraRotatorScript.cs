using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotatorScript : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    private float latestMousePosX, latestMousePosY;
    private float cameraAngle = 55;
    private float maxDist = -85, minDist = -1.6f;
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            latestMousePosX = Input.mousePosition.x;
            latestMousePosY = Input.mousePosition.y;
        }
        if (Input.GetMouseButton(1))
        {
            float deltaX = Input.mousePosition.x - latestMousePosX;
            float deltaY = latestMousePosY - Input.mousePosition.y;
            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                transform.Rotate(0, speed * (Input.mousePosition.x - latestMousePosX), 0);
            }
            else //Доделать проверку вверз-вних
            {
                this.transform.GetChild(0).transform.Rotate(speed * (latestMousePosY - Input.mousePosition.y), 0, 0);
            }
            latestMousePosX = Input.mousePosition.x;
            latestMousePosY = Input.mousePosition.y;
        }

        this.transform.GetChild(0).transform.GetChild(0).localPosition -= new Vector3(0, Input.mouseScrollDelta.y / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), Input.mouseScrollDelta.y);
        Vector3 CameralocalPos = this.transform.GetChild(0).transform.GetChild(0).localPosition;

        if (CameralocalPos.z < maxDist)
            this.transform.GetChild(0).transform.GetChild(0).localPosition = new Vector3(CameralocalPos.x, maxDist / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), maxDist);
        if (CameralocalPos.z > minDist) 
            this.transform.GetChild(0).transform.GetChild(0).localPosition = new Vector3(CameralocalPos.x, minDist / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), minDist);

    }
}
