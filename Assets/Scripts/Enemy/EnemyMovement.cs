using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed;

    private GameObject _player;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = EnemyController.instance._rigidBody;
        _player = EnemyController.instance._playerRigidBody.gameObject;
    }

    public void Move()
    {
        EnemyController.instance._rigidBody.position = Vector2.MoveTowards(_rigidbody.position, _player.transform.position, movementSpeed);
    }

}
