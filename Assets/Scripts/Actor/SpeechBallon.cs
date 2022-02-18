using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpeechBallon : MonoBehaviour
{
    public Text speechField;

    private Animator _animator;

    private void OnEnable()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PopUp(string speech)
    {
        speechField.text = speech;
        _animator.Play("PopUp");
    }
    public void PopDown()
    {
        _animator.Play("PopDown");
    }
}
