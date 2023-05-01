using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] Enemy[] enemies;
	[SerializeField] GameObject roomCam;
	[SerializeField] GameObject[] walls;
	[SerializeField] Animator[] anims;
	private bool startBossFight;
	[SerializeField] int nDefeated;
	[SerializeField] GameObject ui;
	[SerializeField] bool alwaysLockCam;

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
			StartCoroutine( RoomClearedCo() );
		}
	}

	private bool done;
	IEnumerator RoomClearedCo()
	{
		if (done) yield break;

		done = true;
		MusicManager.Instance.PlayMusic(MusicManager.Instance.prevMusic);
		yield return new WaitForSeconds(1);
		// foreach (GameObject wall in walls)
		// 	wall.SetActive(false);
		foreach (Animator anim in anims)
			anim.SetTrigger("open");
		if (!alwaysLockCam)
			roomCam.SetActive(false);
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!startBossFight && other.CompareTag("Player"))
		{
			startBossFight = true;
			Debug.Log("Player Entered Room");
			if (ui != null) ui.SetActive(true);
			
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
		if (done && alwaysLockCam && other.CompareTag("Player"))
		{
			roomCam.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (done && alwaysLockCam && other.CompareTag("Player"))
		{
			roomCam.SetActive(false);
		}
	}
}
