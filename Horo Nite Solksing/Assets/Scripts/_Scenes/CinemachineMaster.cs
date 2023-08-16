using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineMaster : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] vcams;
    [SerializeField] CinemachineShake shakeInstance;
    [SerializeField] CinemachineShake[] vShakes;
	public static CinemachineMaster Instance;
	private Vector3 origOffset;
	public CinemachineVirtualCamera v;

	private void Awake() 
	{
		if (Instance == null)
			Instance = this;	
		else
			Destroy(gameObject);
	}

	private void Start() 
	{
		Debug.Log("New cinemachineMaster");
		SetLiveCinemachineShakeDelay();
	}

	public void SetCinemachineShakeOnHighestPriority()
	{
		v = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
		if (v != null)
		{
			CinemachineShake.Instance = v.gameObject.GetComponent<CinemachineShake>();
			shakeInstance = CinemachineShake.Instance;
			origOffset = v.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
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
			shakeInstance = CinemachineShake.Instance;
			origOffset = v.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
		}
		else
			Invoke("SetCinemachineShakeOnHighestPriority", 0.05f);
	}

	public void SetCamOffset(Vector3 newOffset, float t)
	{
		if (v != null)
			v.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
				Vector3.Lerp(origOffset, origOffset + newOffset, t);
	}
}
