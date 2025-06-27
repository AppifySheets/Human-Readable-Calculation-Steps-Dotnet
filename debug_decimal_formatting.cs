using ArithmeticExpressions;

// Test the decimal formatting issue
var insuranceRate = 0.005m.As("InsuranceRate");
var loanAmount = 50000m.As("LoanAmount");

var annualInsurance = (loanAmount * insuranceRate).As("AnnualInsurance");

Console.WriteLine($"insuranceRate.Value: {insuranceRate.Value}");
Console.WriteLine($"insuranceRate.ToString(): {insuranceRate.Value.ToString()}");
Console.WriteLine($"CleanDecimalFormatting result: {ArithmeticExpressions.VExtensions.CleanDecimalFormatting(insuranceRate.Value.ToString())}");

Console.WriteLine("\nFinalCalculationSteps output:");
Console.WriteLine(annualInsurance.FinalCalculationSteps);