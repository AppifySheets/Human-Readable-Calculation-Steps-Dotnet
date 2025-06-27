# Human Readable Calculation Steps .NET

.NET library designed to dynamically build and evaluate complex mathematical expressions in a structured and transparent way.
It allows you to define variables, construct calculations, and automatically generate step-by-step explanations of how the final result is obtained.

This library is useful for applications where the logic behind a calculation needs to be audited, displayed to an end-user, or debugged.

## Features

- **Share Business Logic with End Users**: Instead of showing your users a final number, show them how you got there. This library lets you share the exact steps of a calculation, which is perfect for building trust and transparency in your applications.
- **Self-Documenting Calculations**: By giving variables and intermediate steps clear, human-readable names, your calculations become self-explanatory. This makes it easier for anyone—from developers to business stakeholders—to understand the logic.
- **Adaptable to Your Business**: Whether you're calculating loan payments, engineering specifications, or complex pricing tiers, this library can model your business logic. You can even use different languages for variable names to make the output more accessible to a global audience.

## Usage

Here are some examples of how to use the `# HumanReadableCalculationSteps` library to build and evaluate complex expressions. 

These examples are taken from the test suite and demonstrate real-world scenarios.

### Financial Calculation: Loan with Multiple Intermediates

This example demonstrates a complex loan calculation with multiple rates and fees.

```csharp
// Complex loan calculation with multiple rates and fees
var loanAmount = 50000m.As("LoanAmount");
var interestRate = 0.045m.As("InterestRate");
var originationFeeRate = 0.01m.As("OriginationFeeRate");
var insuranceRate = 0.005m.As("InsuranceRate");
var years = 5m.As("Years");

var originationFee = (loanAmount * originationFeeRate).As("OriginationFee");
var annualInsurance = (loanAmount * insuranceRate).As("AnnualInsurance");
var totalInsurance = (annualInsurance * years).As("TotalInsurance");
var principalWithFees = (loanAmount + originationFee + totalInsurance).As("PrincipalWithFees");
var totalInterest = (principalWithFees * interestRate * years).As("TotalInterest");
var totalPayment = (principalWithFees + totalInterest).As("TotalPayment");

// The final result is 63393.75m
```

The `totalPayment.FinalCalculationSteps` property will contain the following detailed breakdown of the calculation:

```
OriginationFee = LoanAmount[50,000] * OriginationFeeRate[0.01] = 500

AnnualInsurance = LoanAmount[50,000] * InsuranceRate[0.005] = 250

TotalInsurance = AnnualInsurance[250] * Years[5] = 1,250

PrincipalWithFees = LoanAmount[50,000] + OriginationFee[500] + TotalInsurance[1,250] = 51,750

TotalInterest = PrincipalWithFees[51,750] * InterestRate[0.045] * Years[5] = 11,643.75

TotalPayment = PrincipalWithFees[51,750] + TotalInterest[11,643.75] = 63,393.75
```

### Engineering Calculation: Volume and Density

This example demonstrates an engineering calculation with geometric and material properties.

```csharp
// Engineering calculation with geometric and material properties
var length = 10m.As("Length");
var width = 5m.As("Width");
var height = 3m.As("Height");
var materialDensity = 7.85m.As("SteelDensity"); // kg/m³ (simplified)
var costPerKg = 2.5m.As("CostPerKg");

var area = (length * width).As("Area");
var volume = (area * height).As("Volume");
var mass = (volume * materialDensity).As("Mass");
var totalCost = (mass * costPerKg).As("TotalCost");

// The final result is 2943.75m
```

The `totalCost.FinalCalculationSteps` property will contain the following detailed breakdown of the calculation:

```
Area = Length[10] * Width[5] = 50

Volume = Area[50] * Height[3] = 150

Mass = Volume[150] * SteelDensity[7.85] = 1,177.5

TotalCost = Mass[1,177.5] * CostPerKg[2.5] = 2,943.75
```

### Business Logic: Tiered Discounts

This example demonstrates complex business logic with tiered discounts and multiple customer types.

```csharp
// Complex business logic with tiered discounts and multiple customer types
var basePrice = 1000m.As("BasePrice");
var quantity = 15m.As("Quantity");
var volumeDiscountRate = 0.1m.As("VolumeDiscountRate"); // 10% for >10 items
var loyaltyDiscountRate = 0.05m.As("LoyaltyDiscountRate"); // 5% loyalty discount
var rushOrderSurcharge = 0.15m.As("RushOrderSurcharge"); // 15% rush fee
var taxRate = 0.0875m.As("TaxRate"); // 8.75% tax

var subtotal = (basePrice * quantity).As("Subtotal");
var volumeDiscount = (subtotal * volumeDiscountRate).As("VolumeDiscount");
var afterVolumeDiscount = (subtotal - volumeDiscount).As("AfterVolumeDiscount");
var loyaltyDiscount = (afterVolumeDiscount * loyaltyDiscountRate).As("LoyaltyDiscount");
var afterLoyaltyDiscount = (afterVolumeDiscount - loyaltyDiscount).As("AfterLoyaltyDiscount");
var rushFee = (afterLoyaltyDiscount * rushOrderSurcharge).As("RushFee");
var afterRushFee = (afterLoyaltyDiscount + rushFee).As("AfterRushFee");
var tax = (afterRushFee * taxRate).As("Tax");
var finalPrice = (afterRushFee + tax).As("FinalPrice");

// The final result is 16039.265625m
```

The `finalPrice.FinalCalculationSteps` property will contain the following detailed breakdown of the calculation:

```
Subtotal = BasePrice[1,000] * Quantity[15] = 15,000

VolumeDiscount = Subtotal[15,000] * VolumeDiscountRate[0.1] = 1,500

AfterVolumeDiscount = Subtotal[15,000] - VolumeDiscount[1,500] = 13,500

LoyaltyDiscount = AfterVolumeDiscount[13,500] * LoyaltyDiscountRate[0.05] = 675

AfterLoyaltyDiscount = AfterVolumeDiscount[13,500] - LoyaltyDiscount[675] = 12,825

RushFee = AfterLoyaltyDiscount[12,825] * RushOrderSurcharge[0.15] = 1,923.75

AfterRushFee = AfterLoyaltyDiscount[12,825] + RushFee[1,923.75] = 14,748.75

Tax = AfterRushFee[14,748.75] * TaxRate[0.0875] = 1,290.52

FinalPrice = AfterRushFee[14,748.75] + Tax[1,290.52] = 16,039.27
```

