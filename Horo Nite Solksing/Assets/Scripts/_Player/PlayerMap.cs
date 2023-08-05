using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerMap : MonoBehaviour
{
    [SerializeField] Dictionary<string, GameObject> sceneMap;
	[SerializeField] GameObject[] scenes;
	[SerializeField] RectTransform marker;
	[SerializeField] List<string> existingScenes;
	[SerializeField] List<string> registeredScenes;
	bool setupDone;


	private void Awake() 
	{
		Setup();
	}
	public void Setup()
	{
		if (!setupDone)
		{
			setupDone = true;
			registeredScenes = new List<string>();
			existingScenes = new List<string>();
			if (scenes != null)
			{
				sceneMap = new Dictionary<string, GameObject>();
				foreach (GameObject scene in scenes)
				{
					if (scene != null)
					{
						existingScenes.Add(scene.name);
						sceneMap.Add(scene.name, scene);
						scene.SetActive(false);
					}
				}
			}
		}
	}

	public void CheckForSceneInMap(string sceneName)
	{
		registeredScenes.Add(sceneName);
		if (sceneMap != null && sceneMap.ContainsKey(sceneName))
		{
			sceneMap[sceneName].SetActive(true);
		}
		else
			Debug.Log($"<color=magenta>sceneMap does not contain {sceneName}</color>");
	}

	public void PlaceMarker(string sceneName)
	{
		if (sceneMap != null && sceneMap.ContainsKey(sceneName))
		{
			marker.transform.parent = sceneMap[sceneName].transform.GetChild(0);
			marker.localPosition = Vector3.zero;
			marker.localScale = Vector3.one;
		}
	}

}
