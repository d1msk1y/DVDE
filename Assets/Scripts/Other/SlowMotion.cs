using FMOD;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class SlowMotion : MonoBehaviour {

	[SerializeField] private EventReference _slowMotionSFX;
	private PostProcessingController _postProcessingController => PostProcessingController.instance;

	public static SlowMotion instance;

	private void Awake() {
		instance = this;
	}

	public IEnumerator TriggerSlowMotion(float strength, float time) {
		Time.timeScale = strength;
		SoundManager.PlayOneShot(_slowMotionSFX);
		StartCoroutine(SwitchPostProcessing(0.1f));
		yield return new WaitForSeconds(time);
		Time.timeScale = 1;
	}

	private IEnumerator SwitchPostProcessing(float time) {
		StartCoroutine(_postProcessingController.InterpolateDown(_postProcessingController.DefaultVolume));
		yield return new WaitForSeconds(time);
		StartCoroutine(_postProcessingController.InterpolateUp(_postProcessingController.DefaultVolume));
	}
}