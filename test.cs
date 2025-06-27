using System;
using HumanReadableCalculationSteps;

class Program
{
    static void Main()
    {
        try
        {
            var price1 = 100m.As("price1");
            var taxRate1 = 0.1m.As("taxRate1");
            var tax1 = (price1 * taxRate1).As("Tax1");
            
            Console.WriteLine("Tax1 FinalCalculationSteps:");
            Console.WriteLine(tax1.FinalCalculationSteps);
            Console.WriteLine();
            
            // Test the expected format from the updated test
            var expected = @"Tax1 = 
  price1[100] 
× taxRate1[0.1] 
= 10";
            
            Console.WriteLine("Expected:");
            Console.WriteLine(expected);
            Console.WriteLine();
            
            Console.WriteLine("Match: " + (tax1.FinalCalculationSteps == expected));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}