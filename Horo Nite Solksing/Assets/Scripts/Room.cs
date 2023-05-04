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
	private bool checkedIfCleared;
	[Space] [SerializeField] GameObject otherObj;

	
	[Space] [Header("Waves")] 
	[SerializeField] bool isWaveRoom;
	[SerializeField] int nWaves;
	public int nExtras;
	// [SerializeField] int nSpawnersDefeated;
	[SerializeField] RoomSpawner[] spawners;

	private void Start() 
	{
		// deactivate all walls
		foreach (GameObject wall in walls)
			wall.SetActive(false);

		// has been cleared before
		if (!checkedIfCleared)
		{
			if (CheckedIfCleared() && otherObj != null)
				otherObj.SetActive(true);
		}

		foreach (Enemy enemy in enemies)
		{
			enemy.isWaiting = enemy.cannotAtk = true;	
			enemy.room = this;
		}

		if (isWaveRoom)
		{
			foreach (RoomSpawner spawner in spawners)
				spawner.room = this;
		}
	}

	bool CheckedIfCleared()
	{
		checkedIfCleared = true;
		if (GameManager.Instance.CheckRoomClearedList(gameObject.name))
		{
			startBossFight = done = true;
			foreach (Enemy enemy in enemies)
				Destroy(enemy.gameObject);
			foreach (GameObject wall in walls)
				wall.SetActive(false);
			return true;
		}
		return false;
	}

	public void Defeated()
	{
		nDefeated++;
		Debug.Log($"- Wave {nWaves} = {nDefeated}");
		// no extra spawns
		if (!isWaveRoom)
		{
			if (nDefeated >= enemies.Length)
			{
				StartCoroutine( RoomClearedCo() );
			}
		}
		else
		{
			if (nDefeated >= spawners.Length + nExtras)
			{
				nExtras = 0;
				Debug.Log($"-- Wave {nWaves} cleared");
				nWaves++;
				nDefeated = 0;
				bool stillGoing = false;
				foreach (RoomSpawner spawner in spawners)
				{
					Debug.Log($"--- checking {spawner.name}");
					if (spawner.CheckIfHasMoreSpawns(nWaves))
					{
						Debug.Log($"---- {spawner.name} can still go on");
						stillGoing = true;
						break;
					}
				}
				if (stillGoing)
				{
					StartCoroutine( StartNextWave() );
				}
				else
					StartCoroutine( RoomClearedCo() );
			}
		}
	}

	IEnumerator StartNextWave()
	{
		Debug.Log($"> Starting Wave {nWaves}");
		yield return new WaitForSeconds(1);
		foreach (RoomSpawner spawner in spawners)
		{
			spawner.SpawnEnemy(nWaves);
		}
	}

	private bool done;
	IEnumerator RoomClearedCo()
	{
		if (done) yield break;
		Debug.Log("<color=green>Room Cleared</color>");

		GameManager.Instance.RegisterRoomClearedList(gameObject.name);
		done = true;
		MusicManager.Instance.PlayMusic(MusicManager.Instance.prevMusic);
		yield return new WaitForSeconds(1);
		// foreach (GameObject wall in walls)
		// 	wall.SetActive(false);
		foreach (Animator anim in anims)
			anim.SetTrigger("open");
		if (!alwaysLockCam)
		{
			roomCam.SetActive(false);
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();

		}
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!startBossFight && other.CompareTag("Player"))
		{
			startBossFight = true;
			if (!checkedIfCleared)
			{
				if (CheckedIfCleared())
				{
					return;
				}
			}
			
			if (ui != null) ui.SetActive(true);
			
			if (!isWaveRoom)
			{
				foreach (Enemy enemy in enemies)
				{
					enemy.RoomEnter();
				}
			}
			else
			{
				foreach (RoomSpawner spawner in spawners)
					spawner.SpawnEnemy(nWaves);
			}
			foreach (GameObject wall in walls)
				wall.SetActive(true);

			MusicManager.Instance.PlayMusic(MusicManager.Instance.bossMusic, true);

			roomCam.SetActive(true);
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
			this.enabled = false;
		}
		if (done && alwaysLockCam && other.CompareTag("Player"))
		{
			roomCam.SetActive(true);
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (done && alwaysLockCam && other.CompareTag("Player"))
		{
			roomCam.SetActive(false);
			if (CinemachineMaster.Instance != null) 
				CinemachineMaster.Instance.SetCinemachineShakeOnHighestPriority();
		}
	}
}
