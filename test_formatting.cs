using System;

// Test the decimal formatting issue
var value = 0.005m;
Console.WriteLine($"Original value: {value}");
Console.WriteLine($"ToString(): {value.ToString()}");
Console.WriteLine($"ToString(\"0.##\"): {value.ToString("0.##")}");
Console.WriteLine($"ToString(\"0.###\"): {value.ToString("0.###")}");
Console.WriteLine($"ToString(\"0.####\"): {value.ToString("0.####")}");
Console.WriteLine($"ToString(\"0.#####\"): {value.ToString("0.#####")}");

// Test rounding
Console.WriteLine($"\nRounding behavior:");
Console.WriteLine($"0.005 rounded to 2 decimal places: {Math.Round(0.005m, 2)}");
Console.WriteLine($"0.0045 rounded to 2 decimal places: {Math.Round(0.0045m, 2)}");
Console.WriteLine($"0.0055 rounded to 2 decimal places: {Math.Round(0.0055m, 2)}");