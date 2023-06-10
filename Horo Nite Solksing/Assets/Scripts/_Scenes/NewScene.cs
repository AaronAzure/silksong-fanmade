using UnityEngine;

public class NewScene : MonoBehaviour
{
	[field: SerializeField] public string newSceneName {get; private set;}
	[field: SerializeField] public Vector2 newScenePos {get; private set;}
	[field: SerializeField] public bool isVertical {get; private set;}
}
