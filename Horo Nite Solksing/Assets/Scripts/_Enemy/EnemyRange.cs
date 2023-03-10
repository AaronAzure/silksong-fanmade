using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : MonoBehaviour
{
	[SerializeField] Enemy enemy;


    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && enemy != null)
		{
			enemy.inRange = true;	
			if (enemy.target == null)
				enemy.target = other.GetComponent<PlayerControls>();
		}
	}

    private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && enemy != null)
		{
			enemy.inRange = false;	
		}
	}
}
