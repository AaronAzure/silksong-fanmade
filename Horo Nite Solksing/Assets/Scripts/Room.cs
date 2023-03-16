using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] Enemy[] enemies;
	[SerializeField] GameObject roomCam;
	[SerializeField] GameObject[] walls;

	private void Start() 
	{
		PlayerControls p = GameObject.Find("HORNET (PLAYER)").GetComponent<PlayerControls>();
		foreach (Enemy enemy in enemies)
		{
			enemy.cannotAtk = true;	
			enemy.target = p;
		}
		foreach (GameObject wall in walls)
			wall.SetActive(false);
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Player Entered Room");
			foreach (Enemy enemy in enemies)
			{
				enemy.RoomEnter();	
				enemy.cannotAtk = false;	
			}
			foreach (GameObject wall in walls)
				wall.SetActive(true);

			roomCam.SetActive(true);
			this.enabled = false;
		}
	}
}
