using System.Collections;
using UnityEngine;

public class DiscoReactive : MonoBehaviour
{
    [SerializeField] float updateRate = 0.1f;
    [SerializeField] Vector2 sizeBounds = new Vector2(0, .3f);
    [SerializeField] Material discoMat;
    [Range(0, 1)]
    [SerializeField] float volumeDivider = .5f;
    [SerializeField] float falloffSpeed = .3f;

    private float _volume;
    private static readonly int _CircleSize = Shader.PropertyToID("_CircleSize");
    private float CircleSizeVolume => discoMat.GetFloat(_CircleSize);

    private void Start() {
        StartCoroutine(Reactor());
    }

    private IEnumerator Reactor() {
        var wait = new WaitForEndOfFrame();
        float timer = 0;

        while(true) {
            if(timer > updateRate && GameManager.instance.isCurrentBattle) {
                var tempVolume = GetLevel();
                tempVolume *= volumeDivider;
                tempVolume = Mathf.Clamp(tempVolume, sizeBounds.x, sizeBounds.y);

                if(tempVolume > _volume) {
                    _volume = tempVolume;
                }

                SetCircleSize(_volume);

                timer = 0;
            }

            SizeFalloff();

            timer += Time.deltaTime;
            yield return wait;
        }
    }

    private void Update() {
        if (!GameManager.instance.isCurrentBattle && CircleSizeVolume > 0) {
            SizeFalloff();
        }
    }

    private float GetLevel() {
        var playerBus = FMODUnity.RuntimeManager.GetBus("bus:/OST");
        playerBus.getChannelGroup(out var playerGroup);
        playerGroup.getDSP(0, out var playerdsp);
        playerdsp.setMeteringEnabled(true, true);
        playerdsp.getMeteringInfo(out var ostLevel, out _);
        var level = ostLevel.peaklevel[0] + ostLevel.peaklevel[1];
        return level;
    }

    private void SetCircleSize(float size) => discoMat.SetFloat(_CircleSize, size);

    private void SizeFalloff() {
        if (!(_volume > sizeBounds.x)) return;
        _volume -= falloffSpeed * Time.deltaTime;
        _volume = Mathf.Clamp(_volume, sizeBounds.x, sizeBounds.y);
    }
}