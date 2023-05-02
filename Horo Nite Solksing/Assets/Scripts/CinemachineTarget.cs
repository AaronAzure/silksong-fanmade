// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineTarget : MonoBehaviour
{
	// [SerializeField] 
		
    // Start is called before the first frame update
    void Start()
    {
        CinemachineVirtualCamera cm = this.GetComponent<CinemachineVirtualCamera>();
		if (cm != null)
		{
			cm.m_Follow = PlayerControls.Instance.model;
			// cm.m_Follow = GameObject.Find("HORNET (PLAYER)/Model").transform;
		}
    }
}
