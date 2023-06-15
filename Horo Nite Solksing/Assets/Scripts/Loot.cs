using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] int value=5;
	[SerializeField] Rosary rosary;
	[SerializeField] float xForce=8;
	[SerializeField] float yForce=8;

	public void SpawnLoot()
	{
		for (int i=0 ; i<value ; i++)
		{
			var o = Instantiate(rosary, transform.position, Quaternion.identity);
			o.rb.AddForce(new Vector2(
				Random.Range(-xForce, xForce), 
				yForce
			), ForceMode2D.Impulse);
		}
	}
}
