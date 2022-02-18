using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Vector3 _currentCamPos;

    public void SwitchCamera(Vector3 roomTransform)
    {
        _currentCamPos = new Vector3(roomTransform.x, roomTransform.y, -50);
        transform.position = _currentCamPos;
        //Debug.Log("Camera switched!");
    }
}