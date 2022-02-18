using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class RoomDetection : MonoBehaviour
{
    [SerializeField]private Vector3 pos;
    private void Start()
    {
        pos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameManager.instance.cameraMotor.SwitchCamera(pos);
        }
    }
}
