using UnityEngine;
using TMPro;

public class DmgPopup : MonoBehaviour
{
    public TextMeshPro txt;
    public Animator anim;

	public void DESTROY()
	{
		Destroy(gameObject);
	}
	public void HIDE()
	{
		gameObject.SetActive(false);
	}
}
