using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterUI : MonoBehaviour {
    [SerializeField] private EventReference _typeSound;
    [Header("Parameters")]
    [SerializeField] private float delayBeforeStart;
    [SerializeField] private float timeBtwChars = 0.1f;
    [SerializeField] private string leadingChar = "";
    [SerializeField] private bool leadingCharBeforeDelay;

    private Text _text;
    private TMP_Text _tmpProText;
    private string _initialText;
    private string EndNote => HasEndNote ? " [PRESS SPACE TO CONTINUE]" : "";
    public bool HasEndNote { set; get; }

    public bool IsTextCompleted => _text.text == _initialText + EndNote;

    private void Start() {
        _text = GetComponent<Text>()!;
        _tmpProText = GetComponent<TMP_Text>()!;
        
        TypeText();
    }
    public void TypeText() {
        _initialText = "";
        if (_text != null) {
            _initialText = _text.text;
            _text.text = "";

            StartCoroutine(nameof(TypeWriterText));
        }

        if (_tmpProText != null) {
            _initialText = _tmpProText.text;
            _tmpProText.text = "";

            StartCoroutine(nameof(TypeWriterTMP));
        }
    }

    public void SkipTyping() {
        StopAllCoroutines();
        _text.text = _initialText + EndNote;
    }

    private IEnumerator TypeWriterText() {
        _text.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (var c in _initialText) {
            if (_text.text.Length > 0) {
                _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
            }
            _text.text += c;
            _text.text += leadingChar;
            SoundManager.PlayOneShot(_typeSound);
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "") {
            _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
        }
        yield return new WaitForSeconds(0.3f);
        _text.text += EndNote;
    }

    IEnumerator TypeWriterTMP() {
        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in _initialText) {
            if (_tmpProText.text.Length > 0) {
                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }
            _tmpProText.text += c;
            _tmpProText.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "") {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }
        yield return new WaitForSeconds(0.3f);
        _tmpProText.text += EndNote;
    }
}
