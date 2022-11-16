using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;


public class SoundManager : MonoBehaviour {
    [Header("Audio sources")]
    private EventInstance _soundtrackInstance;
    public EventReference[] soundTracks;
    private int _soundtrackIndex;


    [Header("LowPassFilter properties")]
    [SerializeField] private float _lowPasFilterCutOffSpeed;
    [SerializeField] private bool _isLowPassFilterCutOffDowned;
    private float _lowPassFrequency;
    private float _lowPassTarget;

    [Header("Other")]
    public EventReference hitWall;
    public EventReference pickUp;
    public EventReference actorDeath;
    public EventReference select;

    public static SoundManager instance;
    private float LowPassFrequency {
        get => _lowPassFrequency;
        set {
            _lowPassFrequency = value;
            RuntimeManager.StudioSystem.setParameterByName("Tempo", value);            
        }
    }

    private void Start() {
        instance = this;
        RandomizeTrack();
        StartSoundtrack();

    }


    public static void PlayOneShot(EventReference eventReference) => RuntimeManager.PlayOneShot(eventReference);
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab)) {
            StartSoundtrack();
        }
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L)) {
            SwitchLowPassFrequency();
        }
        #endif
        LowPassFrequency = Mathf.Lerp(LowPassFrequency, _lowPassTarget, _lowPasFilterCutOffSpeed);

        CheckSoundtrack();
    }
    public void SwitchLowPassFrequency() {
        if (_isLowPassFilterCutOffDowned) {
            _lowPassTarget = 1;
            _isLowPassFilterCutOffDowned = false;
        } else {
            _lowPassTarget = 0;
            _isLowPassFilterCutOffDowned = true;
        }
    }
    
    private void CheckSoundtrack() {
        FMOD.Studio.PLAYBACK_STATE state;
        _soundtrackInstance.getPlaybackState(out state);
        if(state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            StartSoundtrack();
    }

    private void RandomizeTrack() {
        var initialSoundTrackIndex = _soundtrackIndex;
        for (var i = 0; initialSoundTrackIndex == _soundtrackIndex;) {
            _soundtrackIndex = Random.Range(0, soundTracks.Length);    
        }
        EventReference soundTrack = soundTracks[_soundtrackIndex];
        _soundtrackInstance = RuntimeManager.CreateInstance(soundTrack);
    }


    private void StartSoundtrack()//Start battle ost n' mute menu ost
    {
        _soundtrackInstance.stop(STOP_MODE.ALLOWFADEOUT);
        RandomizeTrack();
        _soundtrackInstance.start();
    }
}
