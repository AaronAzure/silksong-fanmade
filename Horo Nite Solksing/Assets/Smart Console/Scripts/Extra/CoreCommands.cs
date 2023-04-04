/* 
  Package Name: Smart Console
  Version: 2.0
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-22
  Script Name: CoreCommands.cs

  Description:
  This script implements core commands.
*/

using UnityEngine;

namespace ED.SC.Extra
{
	public static class CoreCommands
	{
		[Command("help", "Displays information about the Smart Console asset")]
		public static void Help()
		{
			SmartConsole.Log("Welcome to Smart Console - a flexible and intuitive in-game command console.\r\nType 'get_commands' for a list of available commands.");
		}

		[Command("get_commands", "Gets available commands")]
		public static void GetCommands()
		{
			string commands = "";

			for (int i = 0; i < Command.Availables.Count; i++)
			{
				Command command = Command.Availables[i];
				commands += $"- {command.Name}";

				if (!string.IsNullOrEmpty(command.Description))
				{
					commands += $": {command.Description}";
				}

				commands += "\r\n";
			}

			SmartConsole.Log($"List of available commands ({Command.Availables.Count}):\r\n{commands}");
		}

		[Command("get_all_commands", "Gets all commands")]
		public static void GetAllCommands()
		{
			string commands = "";

			for (int i = 0; i < Command.All.Count; i++)
			{
				Command command = Command.All[i];
				commands += $"- {command.Name}";

				if (!string.IsNullOrEmpty(command.Description))
				{
					commands += $": {command.Description}";
				}

				commands += "\r\n";
			}

			SmartConsole.Log($"List of all commands ({Command.All.Count}):\r\n{commands}");
		}

		[Command("quit", "Quits the application")]
		public static void Quit()
		{
			Application.Quit();
		}
	}
}
