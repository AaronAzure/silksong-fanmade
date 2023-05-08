using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[field: SerializeField] public Animator transitionAnim {get; private set;}
	[field: SerializeField] public GameObject remapUi {get; private set;}
	[field: SerializeField] public bool showDmg {get; private set;}
	[field: SerializeField] public HashSet<string> shadowRealmed;
	[field: SerializeField] public HashSet<string> roomCleared;


	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		shadowRealmed = new HashSet<string>();
		roomCleared = new HashSet<string>();
	}	

	public void RegisterNameToShadowRealm(string name)
	{
		if (shadowRealmed == null)
			shadowRealmed = new HashSet<string>();

		if (!shadowRealmed.Contains(SceneManager.GetActiveScene().name + " " + name))
			shadowRealmed.Add(SceneManager.GetActiveScene().name + " " + name);
	}
	public bool CheckShadowRealmList(string name)
	{
		if (shadowRealmed == null)
			shadowRealmed = new HashSet<string>();

		return shadowRealmed.Contains(SceneManager.GetActiveScene().name + " " + name);
	}
	public void ClearShadowRealmList()
	{
		if (shadowRealmed == null)
			shadowRealmed = new HashSet<string>();

		shadowRealmed.Clear();
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

		yield return new WaitForSecondsRealtime(0.5f);
		if (PlayerControls.Instance != null)
			Destroy( PlayerControls.Instance.gameObject );

		yield return new WaitForSecondsRealtime(0.5f);
		GameManager.Instance.ClearShadowRealmList();
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
	}

	public void OpenRemapControls()
	{
		if (remapUi != null)
			remapUi.SetActive(true);
	}

	public void CloseRemapControls()
	{
		if (remapUi != null)
			remapUi.SetActive(false);
		if (PlayerControls.Instance != null)
			PlayerControls.Instance.DoneRemapping();
	}
}
