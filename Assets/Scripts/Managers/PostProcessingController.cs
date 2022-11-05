using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcessingController : MonoBehaviour {

	[Header("Volumes")]
	[SerializeField] private PostProcessVolume _slowMotionVolume;
	public PostProcessVolume SlowMotionVolume => _slowMotionVolume;
	[SerializeField] private PostProcessVolume _defaultVolume;
	public PostProcessVolume DefaultVolume => _defaultVolume;

	private float _interpolateVelocityUp;
	private float _interpolateVelocityDown;
	
	public static PostProcessingController instance;

	private void Awake() => instance = this;

	public IEnumerator InterpolatePostProcess (PostProcessVolume a, PostProcessVolume b) {
		StartCoroutine(InterpolateUp(b));
		StartCoroutine(InterpolateDown(a));
		yield return null;
	}

	private IEnumerator InterpolateUp (PostProcessVolume volume) {
		while (volume.weight < 1) {
			volume.weight += 5*Time.deltaTime;
			volume.weight = Mathf.SmoothDamp(volume.weight, 1, ref _interpolateVelocityUp, 0.02f);
			yield return null;
		}
	}
	private IEnumerator InterpolateDown (PostProcessVolume volume) {
		while (volume.weight > 0) {
			volume.weight -= 5*Time.deltaTime;
			// volume.weight = Mathf.SmoothDamp(volume.weight, 0, ref _interpolateVelocityDown, 0.02f);
			yield return null;
		}
	}
	
}

