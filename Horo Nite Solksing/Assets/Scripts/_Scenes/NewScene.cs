using UnityEngine;

public class NewScene : MonoBehaviour
{
	[field: SerializeField] public string newSceneName {get; private set;}
	[field: SerializeField] public bool isVertical {get; private set;}
	[field: SerializeField] public int exitIndex {get; private set;}

	public enum Dir {left=-1, none=0, right=1}
	[SerializeField] Dir dir=0;


	public int GetDirection()
	{
		switch (dir)
		{
			case Dir.left:	return -1;
			case Dir.none:	return 0;
			case Dir.right:	return 1;
			default: 		return 0;
		}
	}
}
