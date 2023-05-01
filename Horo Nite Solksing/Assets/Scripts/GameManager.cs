using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	[field : SerializeField] public bool showDmg {get; private set;}
	public HashSet<string> shadowRealmed;


	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
		shadowRealmed = new HashSet<string>();
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
}
