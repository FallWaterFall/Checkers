using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotatorScript : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    private float latestMousePosX, latestMousePosY;
    private float startScrollDelta;
    Vector2 startTapPos1, startTapPos2;
    private float cameraAngle = 55;
    private float maxDist = -85, minDist = -1.6f;
    private int MoveNum = 0;
    private void Update()
    {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            this.transform.GetChild(0).transform.GetChild(0).localPosition -= new Vector3(0, Input.mouseScrollDelta.y / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), Input.mouseScrollDelta.y);
            //Rotate Camera
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
                else
                {
                    this.transform.GetChild(0).transform.Rotate(speed * (latestMousePosY - Input.mousePosition.y), 0, 0);
                }
                latestMousePosX = Input.mousePosition.x;
                latestMousePosY = Input.mousePosition.y;
            }

        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began))
            {
                startTapPos1 = Input.GetTouch(0).position;
                startTapPos2 = Input.GetTouch(1).position;

                startScrollDelta = Mathf.Abs(startTapPos2.x - startTapPos1.x);
                MoveNum = 0;
            }
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (((Input.GetTouch(0).position.x < startTapPos1.x && Input.GetTouch(1).position.x > startTapPos2.x) || (Input.GetTouch(0).position.x > startTapPos1.x && Input.GetTouch(1).position.x < startTapPos2.x)) && (Mathf.Abs(Input.GetTouch(0).position.x - startTapPos1.x) > Mathf.Abs(Input.GetTouch(0).position.y - startTapPos1.y)) && (MoveNum == 0 || MoveNum == 1))
                {
                    float applyScrollDelta = 0;
                    float newScrollDelta = Mathf.Abs(Input.GetTouch(1).position.x - Input.GetTouch(0).position.x);
                    applyScrollDelta = (startScrollDelta - newScrollDelta) * 0.1f;

                    this.transform.GetChild(0).transform.GetChild(0).localPosition -= new Vector3(0, applyScrollDelta / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), applyScrollDelta);
                    
                    startScrollDelta = newScrollDelta;
                    MoveNum = 1;
                }
                else if (MoveNum == 0 || MoveNum == 2)
                {
                    float deltaX = Input.GetTouch(0).position.x - startTapPos1.x;
                    float deltaY = Input.GetTouch(0).position.y - startTapPos1.y;
                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
                    {
                        transform.Rotate(0, speed * (Input.GetTouch(0).position.x - startTapPos1.x), 0);
                    }
                    else
                    {
                        this.transform.GetChild(0).transform.Rotate(speed * (startTapPos1.y - Input.GetTouch(0).position.y), 0, 0);
                    }
                    MoveNum = 2;
                }
                startTapPos1 = Input.GetTouch(0).position;
                startTapPos2 = Input.GetTouch(1).position;
            }
        }
        //If camera too far or too close
        Vector3 CameralocalPos = this.transform.GetChild(0).transform.GetChild(0).localPosition;
        if (CameralocalPos.z < maxDist)
            this.transform.GetChild(0).transform.GetChild(0).localPosition = new Vector3(CameralocalPos.x, maxDist / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), maxDist);
        if (CameralocalPos.z > minDist) 
            this.transform.GetChild(0).transform.GetChild(0).localPosition = new Vector3(CameralocalPos.x, minDist / Mathf.Tan(cameraAngle / (2.0f * Mathf.PI)), minDist);
        
    }
}
