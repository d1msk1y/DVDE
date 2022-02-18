using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechNPC : Interactable
{
    public SpeechBallon speechBallon;

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
    }

    public override void OnReachZoneEnter()
    {
        if (isActive)
            return;

        isActive = true;

        base.OnReachZoneEnter();

        Say(speechTXT) ;
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
