using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSnap : MonoBehaviour
{
	[SerializeField] CinemachineBrain brain;
	// [SerializeField] CinemachineVirtualCamera vcam;

    private void Start() 
	{
		StartCoroutine( ResetBlendTimeCo() );
	}

	IEnumerator ResetBlendTimeCo()
	{
			if (brain != null)
			{
				brain.m_DefaultBlend.m_Time = 0;
			}
		yield return new WaitForSeconds(0.1f);
		if (brain != null)
		{
			brain.m_DefaultBlend.m_Time = 0.75f;
		}
	}
}
