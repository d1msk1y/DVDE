using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechNPC : Interactable
{
    public SpeechBallon speechBallon;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _speechClips;

    [Multiline]
    public string speechTXT;

    private bool isActive;

    private new void Start()
    {
        base.Start();
        speechBallon = GetComponentInChildren<SpeechBallon>();
    }

    public void Say(string speech)
    {
        speechBallon.PopUp(speech);
        Mumble();
    }

    private void Mumble()
    {
        _audioSource.pitch = GameManager.instance.RandomFloat(1.5f, 2);
        _audioSource.PlayOneShot(_speechClips[GameManager.instance.Chance(0, _speechClips.Length)]);
    }

    public override void OnReachZoneEnter()
    {
        if (isActive)
            return;

        isActive = true;

        base.OnReachZoneEnter();

        Say(speechTXT);
    }

    public override void OnReachZoneExit()
    {
        if (!isActive)
            return;

        isActive = false;

        base.OnReachZoneExit();

        speechBallon.PopDown();
    }
}
