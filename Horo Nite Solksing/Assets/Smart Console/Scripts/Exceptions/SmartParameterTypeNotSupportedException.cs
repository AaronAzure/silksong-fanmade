/*
  Package Name: Smart Console
  Version: 2.0
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: SmartParameterTypeNotSupportedException.cs

  Description:
  Custom exception for handling unsupported parameter types in the Smart Console.
*/

namespace ED.SC
{
	public class SmartParameterTypeNotSupportedException : SmartException
	{
		public SmartParameterTypeNotSupportedException(string parameterTypeName)
		: base($"Parameter type '{parameterTypeName}' is not supported.")
		{
		}
	}
}
