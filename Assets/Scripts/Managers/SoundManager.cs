using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    [Header("Audio sources")]
    public AudioSource _vfxAudioSource;
    public AudioSource _soundtrackAudioSource;
    public AudioSource _OSTAduioSource;
    public AudioSource _glitchSource;

    [Header("Audio properties")]
    public float sfxVolume;

    [Header("LowPassFilter properties")]
    public AudioClip[] soundTracks;
    public AudioClip mainOST;
    [SerializeField] private AudioMixer _ostAudioMixer;

    [Header("LowPassFilter properties")]
    [SerializeField] private AudioLowPassFilter _lowPassFilter;
    [SerializeField] private float _minLowPasFilterCutOff;
    [SerializeField] private float _SoundtrackVolume;
    [SerializeField] private float _lowPasFilterCutOffSpeed;
    [SerializeField] private float _lowPassFreqCurrent;
    [SerializeField] private bool _isLowPassFilterCutOffDowned;

    [Header("Other")]
    public AudioClip withdrawalSound;
    public AudioClip hitWall;
    public AudioClip actorDeath;
    public AudioClip select;
    public AudioClip bulletHit;

    private float _lowPassCutOffVelocityDown;
    private float _lowPassCutOffVelocityUp;

    private void Start()
    {
        PlayMenuOST();
        LowPassFrequencyCutOff();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            RandomizeTrack();
        }
    }

    public void RandomizeTrack()
    {
        _soundtrackAudioSource.clip = soundTracks[Random.Range(0, soundTracks.Length)];

        _soundtrackAudioSource.Play();
    }

    public void StartSoundtrack()//Start battle ost n' mute menu ost
    {
        StartCoroutine(SmoothOnAudio(_soundtrackAudioSource));
        StartCoroutine(SmoothOffAudio(_OSTAduioSource));
        RandomizeTrack();
    }

    public void LowPassFrequencyCutOff()
    {
        if (_isLowPassFilterCutOffDowned)
            StartCoroutine(LowPassFilterUp());
        else if (GameManager.instance.isGameStarted)
            StartCoroutine(LowPassFilterDown());
    }

    public void PlayMenuOST()
    {
        _OSTAduioSource.clip = mainOST;
        StartCoroutine(SmoothOnAudio(_OSTAduioSource));
        StartCoroutine(SmoothOffAudio(_soundtrackAudioSource));
    }

    public IEnumerator SmoothOnAudio(AudioSource audioSource)
    {
        while (audioSource.volume != _SoundtrackVolume)
        {
            audioSource.volume += 1 * Time.deltaTime * 0.9f;
            yield return null;
        }
    }
    public IEnumerator SmoothOffAudio(AudioSource audioSource)
    {
        while (audioSource.volume != 0)
        {
            audioSource.volume -= 1 * Time.deltaTime * 0.9f;
            yield return null;
        }
    }

    private IEnumerator LowPassFilterDown()
    {
        if (_isLowPassFilterCutOffDowned)
            yield return null;

        _isLowPassFilterCutOffDowned = true;

        while (_isLowPassFilterCutOffDowned)
        {
            if (!_isLowPassFilterCutOffDowned)
                yield return null;

            _lowPassFreqCurrent = Mathf.SmoothDamp(22000, _minLowPasFilterCutOff,
                ref _lowPassCutOffVelocityDown, _lowPasFilterCutOffSpeed * Time.deltaTime);
            _ostAudioMixer.SetFloat("lowPassFreq", 1000);
            yield return null;

        }
    }
    private IEnumerator LowPassFilterUp()
    {
        if (!_isLowPassFilterCutOffDowned)
            yield return null;

        _isLowPassFilterCutOffDowned = false;

        while (!_isLowPassFilterCutOffDowned)
        {
            _lowPassFreqCurrent = Mathf.SmoothDamp(_minLowPasFilterCutOff, 22000,
                ref _lowPassCutOffVelocityUp, _lowPasFilterCutOffSpeed * Time.deltaTime);

            _ostAudioMixer.SetFloat("lowPassFreq", 22000);
            yield return null;
        }
    }
}
