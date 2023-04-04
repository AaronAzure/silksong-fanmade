using UnityEngine;
using TMPro;

namespace ED.SC.Components
{
	public class UIFontSetter : MonoBehaviour
	{
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] private TMP_InputField m_InputField;
		[SerializeField] private TextMeshProUGUI[] m_Texts;

		private void Start()
		{
			m_InputField.fontAsset = m_Preferences.GlobalFont;

			foreach (var textMeshPro in m_Texts)
			{
				textMeshPro.font = m_Preferences.GlobalFont;
			}
		}
	}
}