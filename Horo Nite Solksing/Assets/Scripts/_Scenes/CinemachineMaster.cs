using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class CinemachineMaster : MonoBehaviour
{
	public static CinemachineMaster Instance;
	[ReadOnly] public Vector3 origOffset;
	[ShowInInspector] [ReadOnly] public static CinemachineVirtualCamera v;

	private void Awake() 
	{
		if (Instance == null)
			Instance = this;	
		else
			Destroy(gameObject);
	}

	private void Start() 
	{
		SetLiveCinemachineShakeDelay();
	}

	public void SetCinemachineShakeOnHighestPriority()
	{
		v = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
		if (v != null)
		{
			CinemachineShake.Instance = v.gameObject.GetComponent<CinemachineShake>();
		}
		else
			Invoke("SetCinemachineShakeOnHighestPriority", 0.05f);
	}

	public void SetLiveCinemachineShakeDelay()
	{
		StartCoroutine( SetLiveCinemachineShakeDelayCo() );
	}

	public IEnumerator SetLiveCinemachineShakeDelayCo()
	{
		yield return new WaitForSeconds(0.8f);
		v = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
		if (v != null)
		{
			CinemachineShake.Instance = v.gameObject.GetComponent<CinemachineShake>();
		}
		else
			Invoke("SetCinemachineShakeOnHighestPriority", 0.05f);
	}

	public void SetCamOrigOffset(Vector2 newOffset)
	{
		origOffset = new Vector3(0, newOffset.y, -10);
		v.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = origOffset;
	}

	public void SetCamOffset(Vector3 newOffset, float t)
	{
		// if (CinemachineShake.Instance != null && CinemachineShake.Instance.c != null)
		// {
		// 	CinemachineShake.Instance.c.m_Offset = 
		// 		Vector3.Lerp(Vector3.zero, Vector3.zero + newOffset, t);
		// }
		if (v != null)
			v.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
				Vector3.Lerp(origOffset, origOffset + newOffset, Mathf.SmoothStep(0, 1, t));
	}
}
