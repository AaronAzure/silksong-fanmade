using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineOverride : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera newCam;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (newCam != null && other.CompareTag("Player"))	
		{
			newCam.m_Priority = 100;
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
		}
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		if (newCam != null && other.CompareTag("Player"))	
		{
			newCam.m_Priority = -100;
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
		}
	}

}
