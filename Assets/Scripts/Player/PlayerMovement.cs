using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Controls")]
    public Joystick movementJoystick;

    public float movementSpeed;
    public float maxVelocity;

    private Rigidbody2D _rigidBody;

    private Vector2 _moveDirection;
    public Vector2 MoveDirection => _moveDirection;

    private void Start() {
        _rigidBody = PlayerController.instance._rigidBody;
        movementJoystick = GameManager.instance.UiManager.movementJoystick;
    }

    private void Update() {
        _moveDirection.x = Input.GetAxis("Horizontal");
        _moveDirection.y = Input.GetAxis("Vertical");

        if (movementJoystick.Horizontal != 0 || movementJoystick.Vertical != 0) {
            _moveDirection.x = movementJoystick.Horizontal;
            _moveDirection.y = movementJoystick.Vertical;
        }

        _rigidBody.AddForce(MoveDirection*(movementSpeed*Time.deltaTime));

        // if (_rigidBody.velocity != Vector2.zero && moveDirection == Vector2.zero)
        // {
        //     _rigidBody.AddForce(-_rigidBody.velocity * movementSpeed * 0.2f * Time.deltaTime);
        // }

        if (_rigidBody.velocity.magnitude > maxVelocity)
            _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, maxVelocity);
    }

}
