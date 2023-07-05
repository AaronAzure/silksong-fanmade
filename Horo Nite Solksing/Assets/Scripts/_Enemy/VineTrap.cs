using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineTrap : MonoBehaviour
{
	[SerializeField] Collider2D col;
	[SerializeField] GameObject hitbox;

	
	public void _ENABLE_HITBOX()
	{
		if (col != null)
			col.enabled = true;
		if (hitbox != null)
			hitbox.SetActive(true);
	}
}
