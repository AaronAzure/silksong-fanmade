using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class CinemachineShake : MonoBehaviour
{
	[ShowInInspector] public static CinemachineShake Instance;
	public CinemachineVirtualCamera cm;
	private CinemachineBasicMultiChannelPerlin bmcp;
	private float shakeTimer;
	private float shakeTotalTimer;
	private float startingIntensity;
	private bool forever;

	private void Awake() 
	{
		Instance = this;
	}

    // Start is called before the first frame update
    void Start()
    {
		bmcp = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

	public void ShakeCam(float intensity, float duration, float freq=0, bool forever=false)
	{
		if (bmcp != null && intensity > bmcp.m_AmplitudeGain)
		{
			bmcp.m_AmplitudeGain = startingIntensity = intensity;
			bmcp.m_FrequencyGain = freq;
			shakeTimer = shakeTotalTimer = duration;
			this.forever = forever;
		}
	}

	void Update() 
	{
		if (!forever && shakeTimer > 0)
		{
			shakeTimer -= Time.fixedDeltaTime;
			bmcp.m_AmplitudeGain = 
				Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer/shakeTotalTimer));
		}	
	}
}
