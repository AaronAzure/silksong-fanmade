using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void SET_TIME_SCALE(int timeScale)
	{
		Time.timeScale = timeScale;
		if (timeScale == 1 && PlayerControls.Instance != null)
		{
			PlayerControls.Instance.Unpause();
		}
	}
}
