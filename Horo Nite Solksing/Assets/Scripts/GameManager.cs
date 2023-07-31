using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[field: SerializeField] public Animator transitionAnim {get; private set;}
	[field: SerializeField] public GameObject remapUi {get; private set;}
	[field: SerializeField] public GameObject remapCanvasUi {get; private set;}
	[field: SerializeField] public GameObject backdropUi {get; private set;}
	private float vignetteOrigValue;
	[field: SerializeField] public float vignetteMaxValue {get; private set;}=0.55f;
	[field: SerializeField] public bool showDmg {get; private set;}
	[field: SerializeField] public bool easyMode {get; private set;}
	[field: SerializeField] public bool hardMode {get; private set;}
	[field: SerializeField] public float invincibilityDuration {get; private set;}=1f;
	public HashSet<string> roomCleared;
	public HashSet<string> enemiesDefeated;
	public HashSet<string> bossCleared;
	public HashSet<string> destroyedStuff;
	[SerializeField] private List<string> destroyedStuffList; // debug
	[field: SerializeField] public string firstScene="Scene1";


	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}	

	private void Start() 
	{
		destroyedStuffList = new List<string>();
		enemiesDefeated = new HashSet<string>();
		bossCleared = new HashSet<string>();
		roomCleared = new HashSet<string>();
		invincibilityDuration = easyMode ? 1.5f : 1f;
	}

	public void SetFirstSceneName()
	{
		firstScene = SceneManager.GetActiveScene().name;
	}

	// todo --------------------------------------------------------------------
	// todo ----------------------- Enemies ------------------------------------
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

	// todo --------------------------------------------------------------------
	// todo ------------------------ Arenas ------------------------------------
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

	// todo --------------------------------------------------------------------
	// todo ------------------------ Destroyed ---------------------------------
	public void RegisterDestroyedList(string name, bool isSecret=false, bool useExactName=false)
	{
		if (!useExactName)
			name = SceneManager.GetActiveScene().name + " " + name;
		Debug.Log("registered " + name);

		if (isSecret)
		{
			MusicManager.Instance.SoftenBgMusic(3);
			MusicManager.Instance.PlaySecretAreaSFX();
		}

		if (destroyedStuff == null)
			destroyedStuff = new HashSet<string>();

		if (!destroyedStuff.Contains(name))
			destroyedStuff.Add(name);

		// if (!destroyedStuffList.Contains(SceneManager.GetActiveScene().name + " " + name))
		// 	destroyedStuffList.Add(SceneManager.GetActiveScene().name + " " + name);
	}
	public bool CheckDestroyedList(string name, bool useExactName=false)
	{
		if (destroyedStuff == null)
			destroyedStuff = new HashSet<string>();

		if (useExactName)
			return destroyedStuff.Contains(name);

		return destroyedStuff.Contains(SceneManager.GetActiveScene().name + " " + name);
	}
	public void ClearDestroyedList()
	{
		if (destroyedStuff == null)
			destroyedStuff = new HashSet<string>();

		destroyedStuff.Clear();
	}

	// todo --------------------------------------------------------------------
	// todo ------------------------ Bosses ------------------------------------
	public void RegisterBossClearedList(string name)
	{
		if (bossCleared == null)
			bossCleared = new HashSet<string>();

		if (!bossCleared.Contains(SceneManager.GetActiveScene().name + " " + name))
			bossCleared.Add(SceneManager.GetActiveScene().name + " " + name);
	}
	public bool CheckBossClearedList(string name)
	{
		if (bossCleared == null)
			bossCleared = new HashSet<string>();

		return bossCleared.Contains(SceneManager.GetActiveScene().name + " " + name);
	}
	public void ClearBossClearedList()
	{
		if (bossCleared == null)
			bossCleared = new HashSet<string>();

		bossCleared.Clear();
	}


	// todo --------------------------------------------------------------------
	private void ClearAllHashsets()
	{
		ClearBossClearedList();
		ClearEnemiesDefeated();
		ClearRoomClearedList();
		ClearDestroyedList();
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
			PlayerControls.Instance.DestroyItself();

		yield return new WaitForSecondsRealtime(0.5f);
		ClearAllHashsets();

		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(firstScene);
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

	public bool ToggleEasyMode()
	{
		easyMode = !easyMode;
		invincibilityDuration = easyMode ? 1.5f : 1f;
		// if (easyMode)
		// 	invincibilityDuration = 1.25f;
		return easyMode;
	}
	public float ChangeInvincibilityDuration()
	{
		switch (invincibilityDuration)
		{
			case 0.50f:
				invincibilityDuration = 0.75f;
				break;
			case 0.75f:
				invincibilityDuration = 1.00f;
				break;
			case 1.00f:
				invincibilityDuration = 1.25f;
				break;
			case 1.25f:
				invincibilityDuration = 1.5f;
				break;
			case 1.5f:
				invincibilityDuration = 1.75f;
				break;
			case 1.75f:
				invincibilityDuration = 2f;
				break;
			default:
				invincibilityDuration = 0.5f;
				break;
		}
		return invincibilityDuration;
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
			PlayerControls.Instance.DestroyItself();

		yield return new WaitForSecondsRealtime(0.5f);
		ClearAllHashsets();

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

	public void Vignette()
	{
		StartCoroutine( VignetteCo() );
	}

	private IEnumerator VignetteCo()
	{
		if (VignetteScript.Instance != null && VignetteScript.Instance.vignette != null)
		{
			vignetteOrigValue = VignetteScript.Instance.vignette.smoothness.value;
			VignetteScript.Instance.vignette.smoothness.value = vignetteMaxValue;

			float dif = (vignetteMaxValue - vignetteOrigValue)/30;
			for (int i=0 ; i<30; i++)
			{
				yield return new WaitForSeconds(0.01667f);
				if (VignetteScript.Instance != null && VignetteScript.Instance.vignette != null)
				{
					VignetteScript.Instance.vignette.smoothness.value -= dif;
				}
			}
			if (VignetteScript.Instance != null && VignetteScript.Instance.vignette != null)
			{
				VignetteScript.Instance.vignette.smoothness.value = vignetteOrigValue;
			}
		}
	}
}
