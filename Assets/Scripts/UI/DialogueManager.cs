using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class DialogueManager : MonoBehaviour {
	[SerializeField] private Dialogue _introDialogue;
	public static List<Dialogue> dialogues = new();

	public static DialogueManager instance;

	private void Awake() {
		instance = this;
	}

	public void StartIntroDialogue() {
		_introDialogue.gameObject.SetActive(true);
	}

	private void Update() {
		foreach (var dummy in dialogues.Where(dialogue => dialogue.isActiveAndEnabled)) {
			PlayerController.instance.IsInputBlocked = true;
		}
		PlayerController.instance.IsInputBlocked = false;

	}
}

