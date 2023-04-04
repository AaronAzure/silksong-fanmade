/*
  Package Name: Smart Console
  Version: 2.0
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05
  Script Name: SmartParameterInvalidCastException.cs

  Description:
  Custom exception for handling invalid parameters casting in the Smart Console.
*/

namespace ED.SC
{
	public class SmartParameterInvalidCastException : SmartException
	{
		public SmartParameterInvalidCastException(string inputParameter, Command command, string parameterTypeName)
		: base($"Parameter '{inputParameter}' of command '{command.Name}' could not be cast to type '{parameterTypeName}'.")
		{
		}
	}
}
