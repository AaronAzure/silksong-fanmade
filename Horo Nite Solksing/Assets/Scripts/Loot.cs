using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] int value=5;
	[SerializeField] Rosary rosary;

	[Space] [SerializeField] float xForce=8;
	
	[Space] [SerializeField] float yMinForce=6;
	[SerializeField] float yMaxForce=8;

	public void SpawnLoot(int x=-1)
	{
		if (rosary == null)
			return;
		
		if (x == -1)
			x = value;
			
		for (int i=0 ; i<x ; i++)
		{
			var o = Instantiate(rosary, transform.position, Quaternion.identity);
			o.rb.AddForce(new Vector2(
				Random.Range(-xForce, xForce), 
				Random.Range(yMinForce, yMaxForce)
			), ForceMode2D.Impulse);
			o.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
			// o.sr.sortingOrder = i;
		}
	}
}
