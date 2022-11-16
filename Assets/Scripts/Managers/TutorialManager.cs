using System.Collections;
using UnityEngine;
public class TutorialManager : MonoBehaviour {
	[SerializeField] private GameObject[] _popUps;
	private int _popUpIndex;
	private int PopUpIndex {
		get => _popUpIndex;
		set {
			_popUpIndex = value;
			UpdatePopUps();
		}
	}

	private void UpdatePopUps() {
		for (var i = 0; i < _popUps.Length; i++) {
			_popUps[i].SetActive(i == _popUpIndex);
		}
	}

	private void Update() => CheckIndex();

	private void CheckIndex() {
		//WASD
		StartCoroutine(CheckMovementInput());
		//Pickup gun
		StartCoroutine(CheckPickUpInput());
		//Aim shoot
		StartCoroutine(CheckShootingInput());
		//Drop gun
		StartCoroutine(CheckDropInput());
		//Space - Dash
	}
	private IEnumerator CheckMovementInput() {
		var input = Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0;
		if (input || PopUpIndex != 0) yield break;
		yield return new WaitForSeconds(0.5f);
		PopUpIndex++;
	}

	//Player must have no gun before!
	private IEnumerator CheckPickUpInput() {
		var input = Input.GetKeyDown(KeyCode.Mouse0) || PlayerController.instance.shootingScript.gunScript == null;
		if (!input || PopUpIndex != 1) yield break;
		yield return new WaitForSeconds(0.5f);
		PopUpIndex++;
	}

	private IEnumerator CheckShootingInput() {
		var input = Input.GetAxis("Fire1") > 0;
		if(!input || _popUpIndex != 2) yield break;
		yield return new WaitForSeconds(0.5f);
		PopUpIndex++;
	}

	private IEnumerator CheckDropInput() {
		var input = Input.GetKeyDown(KeyCode.Mouse1);
		if (!input || _popUpIndex != 3) yield break;
		yield return new WaitForSeconds(0.5f);
		PopUpIndex++;
	}
}