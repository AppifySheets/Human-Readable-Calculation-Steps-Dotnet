# Human Readable Calculation Steps .NET

Useful for auditing, displaying calculation steps to an end-user.


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