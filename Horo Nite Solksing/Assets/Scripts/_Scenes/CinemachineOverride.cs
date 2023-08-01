using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineOverride : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera newCam;
	[SerializeField] bool canExit=true;
	[SerializeField] bool isSecret;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (newCam != null && other.CompareTag("Player"))	
		{
			if (isSecret)
			{
				GameManager gm = GameManager.Instance;
				if (!gm.CheckDestroyedList(name))
				{
					gm.RegisterDestroyedList(name, true);
				}
				isSecret = false;
			}
			newCam.m_Priority = 100;
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
		}
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		if (canExit && newCam != null && other.CompareTag("Player"))	
		{
			ExitCamLock();
		}
	}

	public void ExitCamLock()
	{
		newCam.m_Priority = -100;
		if (CinemachineMaster.Instance != null) 
			CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
	}
}
