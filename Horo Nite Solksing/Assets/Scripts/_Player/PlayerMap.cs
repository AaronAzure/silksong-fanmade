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


	private void Awake() 
	{
		if (scenes != null)
		{
			sceneMap = new Dictionary<string, GameObject>();
			foreach (GameObject scene in scenes)
				if (scene != null)
					sceneMap.Add(scene.name, scene);
		}
	}

	public void CheckForSceneInMap(string sceneName)
	{
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
			marker.transform.parent = sceneMap[sceneName].transform;
			marker.localPosition = Vector3.zero;
		}
	}

}
