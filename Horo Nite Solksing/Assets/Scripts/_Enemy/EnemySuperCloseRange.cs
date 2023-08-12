using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuperCloseRange : MonoBehaviour
{
    [SerializeField] Enemy enemy;
	[SerializeField] bool reactToTools;
	private bool playerIsSuperClose;
	private bool toolIsSuperClose;
	private int nToolSuperClose;



	private void OnDisable() 
	{
		nToolSuperClose = 0;
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (enemy != null)
		{
			if (other.CompareTag("Player"))
			{
				playerIsSuperClose = enemy.isSuperClose = true;	
				enemy.CallChildOnSuperClose();
			}
			if (reactToTools && other.CompareTag("Respawn"))
			{
				toolIsSuperClose = enemy.toolSuperClose = true;	
				nToolSuperClose++;
				enemy.CallChildOnSuperClose();
			}
		}
	}

    private void OnTriggerExit2D(Collider2D other) 
	{
		if (enemy != null)
		{
			if (other.CompareTag("Player"))
			{
				playerIsSuperClose = false;
				if (!playerIsSuperClose && !toolIsSuperClose)
					enemy.isSuperClose = false;	
			}
			if (reactToTools && other.CompareTag("Respawn"))
			{
				nToolSuperClose--;
				if (nToolSuperClose <= 0)
				{
					toolIsSuperClose = enemy.toolSuperClose = false;	
					if (!playerIsSuperClose && !toolIsSuperClose)
						enemy.ToggleSuperClose(false);
				}
			}
		}
	}
}
