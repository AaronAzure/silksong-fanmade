using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class CinemachineShake : MonoBehaviour
{
	[ReadOnly] [ShowInInspector] public static CinemachineShake Instance;
	[ReadOnly] [ShowInInspector] public static CinemachineFramingTransposer cft;
	public CinemachineVirtualCamera cm;
	private CinemachineBasicMultiChannelPerlin bmcp;
	private float shakeTimer;
	private float shakeTotalTimer;
	private float startingIntensity;
	private bool forever;
	public CinemachineCameraOffset c;

	[Button("Get Cinemachine Camera Offset")]
	public void GetCinemachineCameraOffset()
	{
		if (cm == null)
			cm = GetComponent<CinemachineVirtualCamera>();
		c = GetComponent<CinemachineCameraOffset>();
		if (cm != null)
			cft = cm.GetCinemachineComponent<CinemachineFramingTransposer>();
	}

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

	void FixedUpdate() 
	{
		if (!forever && shakeTimer > 0)
		{
			shakeTimer -= Time.fixedDeltaTime;
			bmcp.m_AmplitudeGain = 
				Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer/shakeTotalTimer));
		}	
	}

	public void _NEW_LIVE_CAM()
	{
		Debug.Log($"<color=yellow>{this.name}</color>");
		CinemachineShake.Instance = this;
	}
}
