using UnityEngine;
public class OstStemController : MonoBehaviour {
	private static FMOD.Studio.EventInstance OstReference => SoundManager.soundtrackEmitter.EventInstance;
	
	public static float Bass { set => OstReference.setParameterByName("Bass", value); }
	public static float Drums { set => OstReference.setParameterByName("Drums", value); }
	public static float Instruments { set => OstReference.setParameterByName("Instruments", value); }
	public static float Melody { set => OstReference.setParameterByName("Melody", value); }
}