using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	[field : SerializeField] public bool showDmg {get; private set;}
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
}
