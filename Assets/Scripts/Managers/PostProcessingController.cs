using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
public class PostProcessingController : MonoBehaviour {

	[Header("Volumes")]
	[SerializeField] private float _interpolationTime;
	[SerializeField] private Volume _slowMotionVolume;
	public Volume SlowMotionVolume => _slowMotionVolume;
	[SerializeField] private Volume _defaultVolume;
	public Volume DefaultVolume => _defaultVolume;

	private float _interpolateVelocityUp;
	private float _weightTarget = 1;
	
	public static PostProcessingController instance;
	

	private void Awake() => instance = this;

	public IEnumerator InterpolatePostProcess (Volume a, Volume b) {
		StartCoroutine(InterpolateUp(b));
		StartCoroutine(InterpolateDown(a));
		yield return null;
	}

	private void Update() {
		_defaultVolume.weight = Mathf.SmoothDamp(_defaultVolume.weight, _weightTarget, ref _interpolateVelocityUp, _interpolationTime);
	}

	public IEnumerator InterpolateUp (Volume volume) {
		_weightTarget = 1;
		yield return null;
	}
	public IEnumerator InterpolateDown (Volume volume) {
		_weightTarget = 0;
		yield return null;
	}
	
}

