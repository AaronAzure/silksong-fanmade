using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteScript : MonoBehaviour
{
    public static VignetteScript Instance;
	public Volume volume;
	public Vignette vignette;

	private void OnEnable() {
		Instance = this;
		volume = GetComponent<Volume>();
		if (volume != null)
		{
			volume.profile.TryGet(out vignette);
			// vignette = volume.GetComponent<Vignette>();
		}
	}
}
