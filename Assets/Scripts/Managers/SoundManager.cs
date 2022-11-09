using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;


public class SoundManager : MonoBehaviour {
    [Header("Audio sources")]
    [SerializeField] private StudioEventEmitter _soundtrackEmitter;
    public static StudioEventEmitter soundtrackEmitter;
    public EventReference[] soundTracks;


    [Header("LowPassFilter properties")]
    [SerializeField] private float _minLowPasFilterCutOff;
    [SerializeField] private float _lowPasFilterCutOffSpeed;
    [SerializeField] private bool _isLowPassFilterCutOffDowned;
    private float _lowPassFrequency;
    private float _lowPassTarget;

    [Header("Other")]
    public EventReference withdrawalSound;
    public EventReference hitWall;
    public EventReference actorDeath;
    public EventReference select;
    public EventReference bulletHit;

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
        soundtrackEmitter = _soundtrackEmitter;
        RandomizeTrack();
        StartSoundtrack();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            StartSoundtrack();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            SwitchLowPassFrequency();
        }
        LowPassFrequency = Mathf.Lerp(LowPassFrequency, _lowPassTarget, _lowPasFilterCutOffSpeed);

        CheckSoundtrack();
    }

    public static void PlayOneShot(EventReference eventReference) => RuntimeManager.PlayOneShot(eventReference);
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
        soundtrackEmitter.EventInstance.getPlaybackState(out state);
        if(state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            StartSoundtrack();
    }

    private void RandomizeTrack() => soundtrackEmitter.EventReference = soundTracks[Random.Range(0, soundTracks.Length)];


    private void StartSoundtrack()//Start battle ost n' mute menu ost
    {
        soundtrackEmitter.Stop();
        RandomizeTrack();
        soundtrackEmitter.Play();
    }
}
