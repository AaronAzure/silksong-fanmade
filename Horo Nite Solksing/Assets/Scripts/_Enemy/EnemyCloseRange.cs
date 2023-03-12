using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseRange : MonoBehaviour
{
	[SerializeField] Enemy enemy;


    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && enemy != null)
		{
			enemy.isClose = true;	
		}
	}

    private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && enemy != null)
		{
			enemy.isClose = false;	
		}
	}
}
