using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuperCloseRange : MonoBehaviour
{
    [SerializeField] Enemy enemy;


    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && enemy != null)
		{
			enemy.isSuperClose = true;	
			enemy.CallChildOnSuperClose();
		}
	}

    private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && enemy != null)
		{
			enemy.isSuperClose = false;	
		}
	}
}
