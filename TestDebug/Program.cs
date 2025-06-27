using System;
using HumanReadableCalculationSteps;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("=== DEBUGGING CalculationSteps_MultipleWrappedValues ===");
            
            var price1 = 100m.As("price1");
            var price2 = 50m.As("price2");
            var taxRate1 = 0.1m.As("taxRate1");
            var taxRate2 = 0.15m.As("taxRate2");
            
            var tax1 = (price1 * taxRate1).As("Tax1");
            var tax2 = (price2 * taxRate2).As("Tax2");
            var totalTax = (tax1 + tax2).As("TotalTax");
            
            Console.WriteLine("=== TAX1 FinalCalculationSteps ===");
            Console.WriteLine("'" + tax1.FinalCalculationSteps + "'");
            Console.WriteLine();
            
            Console.WriteLine("=== TAX2 FinalCalculationSteps ===");
            Console.WriteLine("'" + tax2.FinalCalculationSteps + "'");
            Console.WriteLine();
            
            Console.WriteLine("=== TOTAL TAX FinalCalculationSteps ===");
            Console.WriteLine("'" + totalTax.FinalCalculationSteps + "'");
            Console.WriteLine();
            
            Console.WriteLine("=== EXPECTED ===");
            var expected = @"Tax1 = 
  price1[100] 
× taxRate1[0.1] 
= 10

Tax2 = 
  price2[50] 
× taxRate2[0.15] 
= 7.5

TotalTax =
  Tax1[10] 
+ Tax2[7.5] 
= 17.5";
            Console.WriteLine("'" + expected + "'");
            Console.WriteLine();
            
            Console.WriteLine("=== MATCH ===");
            Console.WriteLine(totalTax.FinalCalculationSteps == expected);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}