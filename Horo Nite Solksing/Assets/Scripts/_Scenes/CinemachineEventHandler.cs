using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineEventHandler : MonoBehaviour
{
	bool on;

    public void SetNewLiveCinemachine()
	{
		Debug.Log(on ? "<color=magenta>New Cinemachine</color>" : "<color=cyan>New Cinemachine</color>");
		on = !on;
		CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
	}
}
