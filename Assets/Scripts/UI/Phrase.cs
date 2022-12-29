using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class Phrase {
	public bool isSkipAllowed = true;
	public string _text;
	public float endDelay = 0.1f;
	public UnityEvent OnStart;
}