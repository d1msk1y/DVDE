using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Dialogue : MonoBehaviour {
	[Header("Parameters")]
	[SerializeField] private Text _text;
	[SerializeField] private List<Phrase> _phrases;
	
	private TypewriterUI _typewriter;
	public int CurrentMessageIndex { get; private set; }
	
	public UnityEvent OnDialogueClose;

	private void Awake() {
		_typewriter = _text.GetComponent<TypewriterUI>();
		_typewriter.HasEndNote = _phrases[CurrentMessageIndex].isSkipAllowed;
	}

	private void Start() {
		DialogueManager.dialogues.Add(this);
	}

	private void OnEnable() {
		StartDialogue();
	}
	private void StartDialogue() {
		PlayerController.instance.IsInputBlocked = true;
		SetText(_phrases[0]._text);
	}

	private void Update() {
		if (_phrases[CurrentMessageIndex].isSkipAllowed && Input.GetKeyDown(KeyCode.Space)) {
			SkipMessage();
		}
	}
	
	public void SkipMessage() {
		if (CurrentMessageIndex >= _phrases.Count -1 && _typewriter.IsTextCompleted) {
			OnDialogueClose?.Invoke();
			Debug.Log("Dialogue ended");
			CloseDialogue();
			return;
		}
		
		if (!_typewriter.IsTextCompleted) {
			_typewriter.SkipTyping();
		} else {
			NextMessage();
		}
	}

	private void NextMessage() {
		CurrentMessageIndex++;
		SetText(_phrases[CurrentMessageIndex]._text);
		_typewriter.HasEndNote = _phrases[CurrentMessageIndex].isSkipAllowed;
	}

	private void CloseDialogue() {
		gameObject.SetActive(false);
		PlayerController.instance.IsInputBlocked = false;
	}

	private void SetText(string message) {
		_text.text = message;
		_typewriter.TypeText();
	}
}