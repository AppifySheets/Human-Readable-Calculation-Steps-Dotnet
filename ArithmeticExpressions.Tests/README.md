# Human Readable Calculation Steps .NET

ArithmeticExpressions is a .NET library designed to dynamically build and evaluate complex mathematical expressions in a structured and transparent way. It allows you to define variables, construct intricate calculations, and automatically generate detailed, step-by-step explanations of how the final result is obtained.

This library is particularly useful for applications where the logic behind a calculation needs to be audited, displayed to an end-user, or debugged. It's ideal for financial, engineering, and business applications where clarity and traceability of calculations are crucial.

## Features

- **Share Business Logic with End Users**: Instead of showing your users a final number, show them how you got there. This library lets you share the exact steps of a calculation, which is perfect for building trust and transparency in your applications.
- **Self-Documenting Calculations**: By giving variables and intermediate steps clear, human-readable names, your calculations become self-explanatory. This makes it easier for anyone—from developers to business stakeholders—to understand the logic.
- **Adaptable to Your Business**: Whether you're calculating loan payments, engineering specifications, or complex pricing tiers, this library can model your business logic. You can even use different languages for variable names to make the output more accessible to a global audience.

## Usage

This example demonstrates how the library automatically handles operator precedence, ensuring that calculations are both accurate and easy to understand. When you combine addition or subtraction with multiplication or division, the library automatically adds parentheses to the output to make the order of operations explicit.

### Operator Precedence

```csharp
var a = 2m.As("a");
var b = 3m.As("b");
var c = 4m.As("c");

// The expression (a + b) * c is evaluated as (2 + 3) * 4 = 20
var result = (a + b) * c;
```

The `result.Caption` property will contain the string `"(a[2] + b[3]) × c[4]"`, showing that addition is performed before multiplication.

### Complex scenarios 

```csharp
var basePrice = 200m.As("BasePrice");
var discountRate = 0.15m.As("DiscountRate");
var taxRate = 0.08m.As("TaxRate");

var discount = (basePrice * discountRate).As("Discount");
var discountedPrice = (basePrice - discount).As("DiscountedPrice");
var tax = (discountedPrice * taxRate).As("Tax");
var finalTotal = (discountedPrice + tax).As("FinalTotal");
```

The `finalTotal.FinalCalculationSteps` property will produce the following output:

```
Discount = BasePrice[200] * DiscountRate[0.15] = 30

DiscountedPrice = BasePrice[200] - Discount[30] = 170

Tax = DiscountedPrice[170] * TaxRate[0.08] = 13.6

FinalTotal = DiscountedPrice[170] + Tax[13.6] = 183.6
```
