using System;
using System.Windows;

public class Program
{
	public static void Main(string[] args)
	{
		AlgorithmsAssignment dcg = new((int)(SystemParameters.PrimaryScreenWidth * 0.9f), (int)(SystemParameters.PrimaryScreenHeight * 0.9f));
		dcg.Start();

		Console.Write("Press enter to close...");
		Console.ReadLine();
	}
}