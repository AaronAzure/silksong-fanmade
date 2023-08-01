using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineExit : MonoBehaviour
{
    [SerializeField] CinemachineOverride master;

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (master != null && other.CompareTag("Player"))	
		{
			master.ExitCamLock();
		}
	}
}
