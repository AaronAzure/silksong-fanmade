using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] Enemy[] enemies;
	[SerializeField] GameObject roomCam;
	[SerializeField] GameObject[] walls;
	private bool startBossFight;
	[SerializeField] int nDefeated;

	private void Start() 
	{
		PlayerControls p = GameObject.Find("HORNET (PLAYER)").GetComponent<PlayerControls>();
		foreach (Enemy enemy in enemies)
		{
			enemy.cannotAtk = true;	
			enemy.target = p;
			enemy.room = this;
		}
		foreach (GameObject wall in walls)
			wall.SetActive(false);
	}

	public void Defeated()
	{
		nDefeated++;
		if (nDefeated >= enemies.Length)
		{
			foreach (GameObject wall in walls)
				wall.SetActive(false);
			roomCam.SetActive(false);
			MusicManager.Instance.PlayMusic(MusicManager.Instance.prevMusic);
		}
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!startBossFight && other.CompareTag("Player"))
		{
			startBossFight = true;
			Debug.Log("Player Entered Room");
			foreach (Enemy enemy in enemies)
			{
				enemy.RoomEnter();	
				// enemy.cannotAtk = false;	
			}
			foreach (GameObject wall in walls)
				wall.SetActive(true);

			MusicManager.Instance.PlayMusic(MusicManager.Instance.bossMusic, true);

			roomCam.SetActive(true);
			this.enabled = false;
		}
	}
}
