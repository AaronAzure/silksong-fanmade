using System;
using UnityEngine;
using UnityEngine.UI;

namespace ED.SC.Components
{
	public class UIBackgroundSetter : MonoBehaviour
	{
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] CanvasGroup m_CanvasGroup;
		[SerializeField] Image[] m_Backgrounds;

		private void Start()
		{
			m_CanvasGroup.alpha = m_Preferences.Opacity;

			foreach (var background in m_Backgrounds )
			{
				background.color = m_Preferences.BackgroundMainColor;
			}
		}
	}
}