using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotator : MonoBehaviour
{
    [SerializeField] private GameObject gunObj;
    [SerializeField] private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Vector3 lookPos = player.transform.position;//Get a player's vector3 variable.
        lookPos -= transform.position;//Calculate the direction from enemy to player.
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;//Geting needed gun rotation angle.
        gunObj.transform.rotation = Quaternion.Euler(0, 0, angle);//Set the gun rotation.
    }
}
