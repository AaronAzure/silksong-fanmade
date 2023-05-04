using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineMaster : MonoBehaviour
{
    // [SerializeField] CinemachineVirtualCamera[] vcams;
    [SerializeField] CinemachineShake[] vShakes;
	public static CinemachineMaster Instance;

	private void Awake() 
	{
		if (Instance == null)
			Instance = this;	
		else
			Destroy(gameObject);
	}

	private void Start() 
	{
		SetCinemachineShakeOnHighestPriority();	
	}

	public void SetCinemachineShakeOnHighestPriority()
	{
		if (vShakes.Length <= 0)
			return;

		int ind = 0;
		int highest = -100000;
		for (int i=0 ; i<vShakes.Length ; i++)
		{
			if (vShakes[i].cm.gameObject.activeInHierarchy && highest < vShakes[i].cm.m_Priority)
			{
				highest = vShakes[i].cm.m_Priority;
				ind = i;
			}
		}
		Debug.Log(vShakes[ind].name);
		CinemachineShake.Instance = vShakes[ind];
	}
}
