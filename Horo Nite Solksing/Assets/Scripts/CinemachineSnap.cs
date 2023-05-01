using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSnap : MonoBehaviour
{
	// [SerializeField] CinemachineBrain brain;
	[SerializeField] CinemachineVirtualCamera vcam;

    private IEnumerator Start() 
	{
		if (vcam != null)
		{
			var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
			transposer.m_XDamping = 0;
			transposer.m_YDamping = 0;
			transposer.m_ZDamping = 0;
			yield return null;
			yield return null;
			transposer.m_XDamping = 1;
			transposer.m_YDamping = 1;
			transposer.m_ZDamping = 1;
		}	
	}
}
