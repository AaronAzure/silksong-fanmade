using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineEventHandler : MonoBehaviour
{
	bool on;
	public static Transform Instance;

	private void Awake() 
	{
		Instance = this.transform;	
	}
	
    public void SetNewLiveCinemachine()
	{
		// Debug.Log(on ? "<color=magenta>New Cinemachine</color>" : "<color=cyan>New Cinemachine</color>");
		on = !on;
		CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
	}
}
