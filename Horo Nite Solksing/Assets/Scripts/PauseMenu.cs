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

	public void _CLOSE_SHOP()
	{
		if (PlayerControls.Instance != null)
		{
			PlayerControls.Instance.Unpause();
		}
	}

	public void _DEACTIVATE_SHOP_CAM()
	{
		if (PlayerControls.Instance != null)
		{
			PlayerControls.Instance.HideShopCam();
		}
	}
}
