using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[field: SerializeField] public Animator transitionAnim {get; private set;}
	[field: SerializeField] public GameObject remapUi {get; private set;}
	[field: SerializeField] public GameObject remapCanvasUi {get; private set;}
	[field: SerializeField] public GameObject backdropUi {get; private set;}
	[field: SerializeField] public bool showDmg {get; private set;}
	[field: SerializeField] public HashSet<string> roomCleared;
	[field: SerializeField] public HashSet<string> enemiesDefeated;


	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		enemiesDefeated = new HashSet<string>();
		roomCleared = new HashSet<string>();
	}	

	public void RegisterNameToEnemiesDefeated(string name)
	{
		if (enemiesDefeated == null)
			enemiesDefeated = new HashSet<string>();

		if (!enemiesDefeated.Contains(SceneManager.GetActiveScene().name + " " + name))
			enemiesDefeated.Add(SceneManager.GetActiveScene().name + " " + name);
	}

	public bool CheckDivineHashmapIfNameIsRegistered(string name)
	{
		return enemiesDefeated.Contains(SceneManager.GetActiveScene().name + " " + name);
	}

	public void ClearEnemiesDefeated()
	{
		if (enemiesDefeated == null)
			enemiesDefeated = new HashSet<string>();

		enemiesDefeated.Clear();
	}

	public void RegisterRoomClearedList(string name)
	{
		if (roomCleared == null)
			roomCleared = new HashSet<string>();

		if (!roomCleared.Contains(SceneManager.GetActiveScene().name + " " + name))
			roomCleared.Add(SceneManager.GetActiveScene().name + " " + name);
	}
	public bool CheckRoomClearedList(string name)
	{
		if (roomCleared == null)
			roomCleared = new HashSet<string>();

		return roomCleared.Contains(SceneManager.GetActiveScene().name + " " + name);
	}
	public void ClearRoomClearedList()
	{
		if (roomCleared == null)
			roomCleared = new HashSet<string>();

		roomCleared.Clear();
	}

	public void Restart()
	{
		StartCoroutine( RestartCo() );
	}

	IEnumerator RestartCo()
	{
		transitionAnim.SetTrigger("toBlack");
		MusicManager m = MusicManager.Instance;
		m.PlayMusic(null, 0);

		yield return new WaitForSecondsRealtime(0.5f);
		if (PlayerControls.Instance != null)
			Destroy( PlayerControls.Instance.gameObject );

		yield return new WaitForSecondsRealtime(0.5f);
		GameManager.Instance.ClearEnemiesDefeated();
		GameManager.Instance.ClearRoomClearedList();

		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Scene1");
		Time.timeScale = 1;
		float loadTime = 0;
		// wait for scene to load
		while (!loadingOperation.isDone && loadTime < 5)
		{
			loadTime += Time.deltaTime;
			yield return null;
		}
		transitionAnim.SetTrigger("reset");
		// m.PlayMusic(m.bgMusic, m.bgMusicVol);
	}

	public void OpenRemapControls()
	{
		if (remapUi != null)
			remapUi.SetActive(true);
		StartCoroutine( ActivateRemapCanvasCo() );
	}

	IEnumerator ActivateRemapCanvasCo()
	{
		yield return null;
		if (remapCanvasUi != null)
			remapCanvasUi.SetActive(true);
	}

	public void CloseRemapControls()
	{
		if (remapUi != null)
			remapUi.SetActive(false);
		if (PlayerControls.Instance != null)
			PlayerControls.Instance.DoneRemapping();
		if (UiTitleButton.Instance != null)
			UiTitleButton.Instance.DoneRemapping();
	}

	public bool ToggleDmgIndicator()
	{
		showDmg = !showDmg;
		return showDmg;
	}

	public void ExitGame()
	{
		StartCoroutine( ExitGameCo() );
	}

	IEnumerator ExitGameCo()
	{
		transitionAnim.SetTrigger("toBlack");
		MusicManager m = MusicManager.Instance;
		m.PlayMusic(null, 0);

		yield return new WaitForSecondsRealtime(0.5f);
		if (PlayerControls.Instance != null)
			Destroy( PlayerControls.Instance.gameObject );

		yield return new WaitForSecondsRealtime(0.5f);
		GameManager.Instance.ClearEnemiesDefeated();
		GameManager.Instance.ClearRoomClearedList();

		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("0TitleScreen");
		Time.timeScale = 1;
		float loadTime = 0;
		// wait for scene to load
		while (!loadingOperation.isDone && loadTime < 5)
		{
			loadTime += Time.deltaTime;
			yield return null;
		}
		transitionAnim.SetTrigger("reset");
		// m.PlayMusic(m.bgMusic, m.bgMusicVol);
	}

	// public void ResumeTime()
	// {
	// 	if (!isPaused)
	// 		Time.timeScale = 1;
	// }
}
