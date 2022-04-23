using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Controlls")]
    public Joystick movementJoystick;

    public float movementSpeed;
    public float maxMovementSpeed;

    public float dodgeForce;
    public float dodgeRechargeTime;
    public float dodgeDistance;
    public float dodgeCount;

    public float currentDistance;
    Vector3 startPos;

    public Vector2 moveDirection;

    Rigidbody2D _rigidBody;

    private void Start()
    {
        _rigidBody = PlayerController.instance._rigidBody;
        movementJoystick = GameManager.instance.UiManager.movementJoystick;
    }

    private void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");

        if (movementJoystick.Horizontal != 0 || movementJoystick.Vertical != 0)
        {
            moveDirection.x = movementJoystick.Horizontal;
            moveDirection.y = movementJoystick.Vertical;
        }

        _rigidBody.AddForce(moveDirection * movementSpeed * Time.deltaTime);

        currentDistance = Vector3.Distance(transform.position, startPos);

        if (_rigidBody.velocity != Vector2.zero && moveDirection == Vector2.zero)
        {
            _rigidBody.AddForce(-_rigidBody.velocity * movementSpeed * 0.2f * Time.deltaTime);
        }

    }

    public void Dodge()
    {
        if (moveDirection == Vector2.zero)
            return;

        _rigidBody.velocity = moveDirection * Vector2.one * dodgeDistance;
    }

}
