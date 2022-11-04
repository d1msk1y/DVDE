using System.Collections;
using UnityEngine;
public class RigidBodyDash : MonoBehaviour, IAbility {

	[Header("Parameters")]
	[SerializeField] private float _force;
	[SerializeField] private float _transparencyTime;
	[SerializeField] private float _rechargeTime;
	
	[Space(10)]
	[SerializeField] private Rigidbody2D _rigidbody;

	private float _timer;
	private static Vector2 DashDirection => PlayerController.instance.playerMovement.MoveDirection;
	public bool IsRecharged() => _timer <= 0;


	//IAbility//
	public void TriggerAction() {
		TryDash();		
	}

	private void Update() {
		if (!IsRecharged())
			_timer -= Time.deltaTime;
	}
	
	private void TryDash() {
		if (!IsRecharged())
			return;

		Dash();
		StartCoroutine(SwitchLayers());
		_timer = _rechargeTime;
	}
	private void Dash() {
		_rigidbody.AddForce(DashDirection* _force, ForceMode2D.Impulse);
	}

	private IEnumerator SwitchLayers() {
		var initialLayer = gameObject.layer;
		gameObject.layer = 16;
		yield return new WaitForSeconds(_transparencyTime);
		gameObject.layer = initialLayer;
	}
}