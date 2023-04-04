/*
  Package Name: Smart Console
  Version: 2.0
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-03
  Script Name: SmartConsole.cs

  Description:
  This script is the main entry point for actions and callbacks.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace ED.SC
{
	public static class SmartConsole
	{
		/// <summary>
		/// Delegate method that takes in a LogMessage object and returns nothing.
		/// This is used to send a LogMessage object whenever a new log is added to the console.
		/// </summary>
		internal delegate void LogMessageCallback(LogMessage logMessage);

		/// <summary>
		/// Delegate method that takes no parameters and returns nothing.
		/// This is used to clear the console.
		/// </summary>
		internal delegate void OnClearCallback();

		/// <summary>
		/// Delegate method that takes no parameters and returns nothing.
		/// This is used to reset the console to its default state.
		/// </summary>
		internal delegate void OnResetCallback();

		/// <summary>
		/// Delegate method that takes in a string and returns nothing.
		/// This is used to handle input text submitted by the user.
		/// </summary>
		internal delegate void OnSubmitCallback(string inputText);

		/// <summary>
		/// This event is raised whenever a new log is added to the console.
		/// It is subscribed by the console to display the new log in the console.
		/// </summary>
		internal static event LogMessageCallback LogMessageReceived;

		/// <summary>
		/// This event is raised whenever a new log is added to the console.
		/// It is subscribed by the console to display the new autocompletion log in the console.
		/// </summary>
		internal static event LogMessageCallback AutocompletionLogMessageReceived;

		/// <summary>
		/// This event is raised whenever the console is cleared.
		/// It is subscribed by the console to clear the console display.
		/// </summary>
		internal static event OnClearCallback OnClear;

		/// <summary>
		/// This event is raised whenever the console is cleared.
		/// It is subscribed by the console to clear the console's autocompletions display.
		/// </summary>
		internal static event OnClearCallback OnClearAutocompletions;

		/// <summary>
		/// This event is raised whenever the console is reset to its default state.
		/// It is subscribed by the console to reset the console display.
		/// </summary>
		internal static event OnResetCallback OnReset;

		/// <summary>
		/// This event is raised whenever the user submits input text.
		/// It is subscribed by the console to handle the input text.
		/// </summary>
		internal static event OnSubmitCallback OnSubmit;

		internal static readonly List<LogMessage> LogMessages = new List<LogMessage>();
		internal static readonly List<LogMessage> AutocompletionLogMessages = new List<LogMessage>();
		internal static readonly List<LogMessage> LogMessagesHistory = new List<LogMessage>();
		internal static TMP_InputField InputField;

		/// <summary>
		/// This function is used to initialize the console.
		/// It is used in the system script at it start.
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			// find methods in all assemblies that has the [Command] attribute
			List<MethodInfo> methodInfos = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.SelectMany(type => type.GetMethods(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Static |
					BindingFlags.Instance))
				.Where(method => method.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
				.ToList();

			foreach (var method in methodInfos)
			{
				CommandAttribute attribute = Attribute.GetCustomAttribute(method, typeof(CommandAttribute)) as CommandAttribute;
				string commandName = attribute.HasName() ? attribute.Name : method.Name;
				commandName = commandName.Replace(" ", "");

				if (Command.All.Exists(cmd => string.Equals(cmd.Name, commandName)))
				{
					// this command name is not available
					Debug.LogWarning($"Command '{commandName}' could not be added because its name is duplicated.");
				}
				else
				{
					// this command name is available
					Command command = new Command(commandName, attribute.Description, attribute.MonoTargetType, method);
					Command.All.Add(command);
				}
			}

			// sort all the commands for the autocompletion
			Command.All.Sort((cmd1, cmd2) => cmd1.Name.CompareTo(cmd2.Name));

			RefreshAutocomplete();

			Log("Smart Console has been setup successfully.");
		}

		/// <summary>
		/// This function is used to refresh the autocomplete suggestions.
		/// It should be used when a new command has been added or removed at run time.
		/// </summary>
		public static void RefreshAutocomplete()
		{
			if (Command.Availables.Count > 0)
			{
				Command.Availables.Clear();
			}

			// find command that are currently available
			IEnumerable<Command> availableCommands = Command.All.Where(cmd =>
			{
				if (cmd.MethodInfo.IsStatic)
				{
					return true;
				}

				Type targetType = cmd.MethodInfo.ReflectedType;
				object target = GameObject.FindObjectOfType(targetType);

				return target != null;
			});

			// add it to the global list
			Command.Availables.AddRange(availableCommands);
		}

		/// <summary>
		/// Adds a log to the console.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void Log(string message)
		{
			var logMessage = new LogMessage(message, LogMessageType.Log);
			LogMessageReceived?.Invoke(logMessage);
		}

		/// <summary>
		/// Adds a warning log to the console.
		/// </summary>
		/// <param name="message">The warning to log.</param>
		public static void LogWarning(string message)
		{
			var logMessage = new LogMessage(message, LogMessageType.Warning);
			LogMessageReceived?.Invoke(logMessage);
		}

		/// <summary>
		/// Adds a error log to the console.
		/// </summary>
		/// <param name="message">The error to log.</param>
		public static void LogError(string message)
		{
			var logMessage = new LogMessage(message, LogMessageType.Error);
			LogMessageReceived?.Invoke(logMessage);
		}

		/// <summary>
		/// Adds a command message to the console.
		/// </summary>
		/// <param name="commandMessage">The command message to log.</param>
		internal static void LogCommand(LogMessage commandMessage)
		{
			LogMessageReceived?.Invoke(commandMessage);
		}

		/// <summary>
		/// Adds an autocompletion log message to the console.
		/// </summary>
		/// <param name="autocompletionLogMessage">The autocompletion message to log.</param>
		internal static void LogAutocompletion(LogMessage autocompletionLogMessage)
		{
			AutocompletionLogMessageReceived?.Invoke(autocompletionLogMessage);
		}

		/// <summary>
		/// Sends a message to the console using its input.
		/// </summary>
		/// <param name="message">The message to send.</param>
		public static void Send(string message)
		{
			SetInputText(message, false, false);
			SubmitInputField();
		}

		/// <summary>
		/// Gets the current input text.
		/// </summary>
		/// <returns>the input text</returns>
		internal static string GetInputText() => InputField.text;

		/// <summary>
		/// Sets the input text content.
		/// </summary>
		/// <param name="value">the text content</param>
		/// <param name="notify">notifies input to get autocompletions</param>
		/// <param name="moveCaret">moves the caret at the text content's end</param>
		internal static void SetInputText(string value, bool notify = false, bool moveCaret = true)
		{
			if (notify)
			{
				InputField.text = value;
			}
			else
			{
				InputField.SetTextWithoutNotify(value);
			}

			if (moveCaret)
			{
				MoveCaret();
			}
		}

		internal static void ActivateInputField()
		{
			InputField.ActivateInputField();
		}

		internal static void SubmitInputField()
		{
			OnSubmit?.Invoke(GetInputText());
		}

		/// <summary>
		/// Move caret to end of text after a frame
		/// </summary>
		internal static IEnumerator MoveCaretCoroutine()
		{
			yield return null;
			InputField.MoveTextEnd(false);
		}

		/// <summary>
		/// Move caret to end of text
		/// </summary>
		private static void MoveCaret() => InputField.MoveTextEnd(false);

		/// <summary>
		/// Clears the console.
		/// </summary>
		[Command("clear", "Clears the console")]
		public static void Clear()
		{
			if (LogMessages.Count == 0)
			{
				return;
			}

			for (int i = 0; i < LogMessages.Count; i++)
			{
				GameObject.Destroy(LogMessages[i].LogMessageSetup.gameObject);
			}

			LogMessages.Clear();
			OnClear?.Invoke();
		}

		/// <summary>
		/// Clears the console's autocompletions.
		/// </summary>
		internal static void ClearAutocompletions()
		{
			if (AutocompletionLogMessages.Count == 0)
			{
				return;
			}

			for (int i = 0; i < AutocompletionLogMessages.Count; i++)
			{
				GameObject.Destroy(AutocompletionLogMessages[i].LogMessageSetup.gameObject);
			}

			AutocompletionLogMessages.Clear();
			OnClearAutocompletions?.Invoke();
		}

		/// <summary>
		/// Resets the console to its default state.
		/// </summary>
		internal static void Reset()
		{
			SetInputText("", false, false);
			ClearAutocompletions();

			ActivateInputField();

			OnReset?.Invoke();
		}

		/// <summary>
		/// Gets all logs of the console.
		/// </summary>
		public static LogMessage[] GetLogs()
		{
			return LogMessages.ToArray();
		}

		/// <summary>
		/// Gets user's history logs of the console from the start of the application
		/// </summary>
		public static LogMessage[] GetHistoryLogs()
		{
			return LogMessagesHistory.ToArray();
		}
	}
}
