using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mathias.Utilities
{
	/// <summary>
	///     A helper class to make logging information to the console easier.
	/// </summary>
	public static class Debug
	{
		/// <summary>
		///     Log a <paramref name="message" /> to the console with a red "[ERROR]" tag in front of it.
		/// </summary>
		/// <param name="message">The text that has to be displayed in the console</param>
		public static void LogError(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callerPath = null)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			string fileName = callerPath.Split('\\').Last();
			Console.Write($"[ERROR {fileName}:{lineNumber}] ");
			Console.ResetColor();
			Console.WriteLine(message);
		}

		/// <summary>
		///     Log a <paramref name="message" /> to the console with a gray-ish "[INFO]" tag in front of it.
		/// </summary>
		/// <param name="message">The text that has to be displayed in the console</param>
		public static void Log(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callerPath = null)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			string fileName = callerPath.Split('\\').Last();
			Console.Write($"[INFO {fileName}:{lineNumber}] ");
			Console.ResetColor();
			Console.WriteLine(message);
		}

		/// <summary>
		///     Log a <paramref name="message" /> to the console with a yellow "[WARNING]" tag in front of it.
		/// </summary>
		/// <param name="message">The text that has to be displayed in the console</param>
		public static void LogWaring(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callerPath = null)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string fileName = callerPath.Split('\\').Last();
			Console.Write($"[WARNING {fileName}:{lineNumber}] ");
			Console.ResetColor();
			Console.WriteLine(message);
		}

		/// <summary>
		///     Log a <paramref name="message" /> with a blue "[INITIALIZED]" tag in front of it and the
		///     <paramref name="elapsedTime" /> in milliseconds it took to initialize afterwards.
		/// </summary>
		/// <param name="message">The text that has to be displayed in the console</param>
		/// <param name="elapsedTime">The time it took to initialize the object.</param>
		public static void Initialized(object initializedObject, double elapsedTime)
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("[INITIALIZED] ");
			Console.ResetColor();
			Console.WriteLine($"{initializedObject.GetType().Name} in {elapsedTime}ms...");
		}

		/// <summary>
		///     Log a <paramref name="message" /> to the console with a DarkMagenta "[UNIT-TEST]" tag in front of it and the show
		///     in color whether the unit test has passed or failed.
		/// </summary>
		/// <param name="message">The text that has to be displayed in the console</param>
		public static void UnitTest(string message, bool condition)
		{
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.Write("[UNIT-TEST] ");
			Console.ResetColor();
			Console.Write(message);

			var result = string.Empty;

			if (condition)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				result = "Passed!";
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				result = "Failed!";
			}

			const int standardValuesLength = 20;
			var whitespace = new string(' ', Console.WindowWidth - message.Length - standardValuesLength);
			Console.WriteLine(whitespace + result);

			Console.ResetColor();
		}
	}
}