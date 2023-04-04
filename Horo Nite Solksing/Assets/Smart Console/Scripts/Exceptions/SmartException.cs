/*
  Package Name: Smart Console
  Version: 2.0
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: SmartException.cs

  Description:
  Base exception class for Smart Console.
*/

using System;

namespace ED.SC
{
	public class SmartException : Exception
	{
		public SmartException(string message)
		: base($"Smart Error: {message}")
		{
		}
	}
}
