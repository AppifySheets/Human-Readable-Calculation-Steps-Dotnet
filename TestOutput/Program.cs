using ArithmeticExpressions;

// Test the decimal formatting and spacing
var insuranceRate = 0.005m.As("InsuranceRate");
var loanAmount = 50000m.As("LoanAmount");

var annualInsurance = (loanAmount * insuranceRate).As("AnnualInsurance");

Console.WriteLine("=== Basic Values ===");
Console.WriteLine($"insuranceRate: {insuranceRate}");
Console.WriteLine($"loanAmount: {loanAmount}");

Console.WriteLine($"\n=== FinalCalculationSteps Output ===");
Console.WriteLine(annualInsurance.FinalCalculationSteps);