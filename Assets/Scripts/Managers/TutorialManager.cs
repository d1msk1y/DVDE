using System.Collections;
using UnityEngine;
public class TutorialManager : MonoBehaviour {
	[SerializeField] private Dialogue _dialogue;

	private int PopUpIndex => _dialogue.CurrentMessageIndex;
	private bool _isSwitching;
	private bool _isStarted;

	private void NextMessage() => _dialogue.SkipMessage();

	public void StartTutorial() {
		if (GameManager.instance.IsFirstTime == 0) {
			return;
		}
		_isStarted = true;
	}
	private void Update() {
		if (_isSwitching || !_isStarted) {
			return;
		}
		CheckMovementInput();
		CheckPickUpInput();
		CheckShootingInput();
		CheckDropInput();
	}

	private void NextStep() {
		if (_isSwitching) {
			return;
		}
		StartCoroutine(DelaySwitching());
		NextMessage();
	}

	private IEnumerator DelaySwitching() {
		_isSwitching = true;
		Debug.Log("Switching");
		yield return new WaitForSeconds(1f);
		_isSwitching = false;
		StartTutorial();
	}

	#region Step Checks
	
	private void CheckMovementInput() {
		var input = Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0;
		if (input || PopUpIndex != 0) return;
		NextStep();
	}

	//Player must have no gun before!
	private void CheckPickUpInput() {
		var input = Input.GetKeyDown(KeyCode.Mouse1) || PlayerController.instance.shootingScript.gunScript == null;
		if (!input || PopUpIndex != 1) return;
		NextStep();
	}

	private void CheckShootingInput() {
		var input = Input.GetAxis("Fire1") > 0;
		if(!input || PopUpIndex != 2) return;
		NextStep();
	}

	private void CheckDropInput() {
		var input = Input.GetKeyDown(KeyCode.Mouse1);
		if (!input || PopUpIndex != 3) return;
		NextStep();
	}

	#endregion
}