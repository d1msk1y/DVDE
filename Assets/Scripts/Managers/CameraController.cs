using UnityEngine;

public class CameraController : MonoBehaviour {
	[Header("Bounds")]
	[Range(0, 5)][SerializeField] private float _xRange;
	[Range(0, 5)][SerializeField] private float _yRange;

	[Header("Parameters")]
	[Range(0.001f, 0.1f)][SerializeField] private float _transitionSpeed;
	[Range(0.1f, 1)][SerializeField] private float _sensivity;

	private void Update() {
		transform.position = Vector2.Lerp(transform.position, GetProcessedMousePos(), _transitionSpeed);
	}

	private Vector3 GetProcessedMousePos() {
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos -= PlayerController.instance._rigidBody.position;
		var x = Mathf.Clamp(mousePos.x, -_xRange, _xRange);
		var y = Mathf.Clamp(mousePos.y, -_yRange, _yRange);
		mousePos = new Vector2(x, y);

		return mousePos;
	}
}
