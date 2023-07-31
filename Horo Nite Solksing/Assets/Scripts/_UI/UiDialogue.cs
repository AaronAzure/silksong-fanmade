using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiDialogue : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI txt;
	[SerializeField] float textSpeed=0.05f;
	[SerializeField] Animator dialogueAnim;
	[Space] [SerializeField] string[] lines;
	[Space] [SerializeField] string[] defaultText;
	private int index=0;
	private bool closing;


	private void Awake() 
	{
		if (txt == null)
			txt = GetComponent<TextMeshProUGUI>();	
		StartDialogue();
	}

	private void OnEnable() 
	{
		closing = false;
		StartDialogue();
		StartCoroutine( TypeLine() );
	}

	public void SetLines(string[] newLines)
	{
		lines = newLines;
	}

    void StartDialogue()
	{
		txt.text = "";
	}

	public void NextLine()
	{
		// next line
		if (txt.text == lines[index])
		{
			if (index < lines.Length - 1)
			{
				index++;
				StartDialogue();
				StartCoroutine( TypeLine() );
			}
			else if (!closing && dialogueAnim != null)
			{
				closing = true;
				dialogueAnim.SetTrigger("close");
			}
		}
		// fast forward
		else
		{
			StopAllCoroutines();
			txt.text = lines[index];
		}
	}

	IEnumerator TypeLine()
	{
		foreach (char c in lines[index].ToCharArray())
		{
			txt.text += c;
			yield return new WaitForSeconds(textSpeed);
		}
	}

	public bool IsDefaultText()
	{
		return lines == defaultText;
	}

	public void _CLOSE()
	{
		gameObject.SetActive(false);
		if (PlayerControls.Instance != null)
		{
			PlayerControls.Instance.ToggleShop(true);
		}
		lines = defaultText;
		index = 0;
	}
}