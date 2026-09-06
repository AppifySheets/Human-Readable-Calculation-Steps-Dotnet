# Human Readable Calculation Steps

A .NET library for arithmetic that explains itself. Every value carries a caption, every operator records its derivation, and the final result can be printed as the human-readable steps that produced it.

Useful for invoices, payroll, tax breakdowns, financial reports, or any UI where a number on screen needs to explain where it came from.

## Install

```bash
dotnet add package Appify.HumanReadableCalculationSteps
```

Target framework: `.NET 8.0`.

## Quick start

```csharp
using HumanReadableCalculationSteps;

var a = 2m.As("a");
var b = 3m.As("b");
var c = 4m.As("c");

var result = (a + b) * c;

// result.Value                  -> 20
// result.FinalCalculationSteps  -> "(a[2] + b[3]) × c[4] = 20"
```

The `× ` and `÷` symbols are emitted instead of `*` and `/`, and parentheses are inserted automatically where operator precedence demands them.

---

## Features

Each section is one self-contained way to use the library, with a runnable example and the exact output you'd see in `FinalCalculationSteps`.

### 1. Captioning literals — `.As(caption)`

Attach a human-readable name to any `decimal` or `int`.

```csharp
var price    = 100m.As("Price");
var quantity = 3.As("Quantity");

// price.FinalCalculationSteps    -> "Price = 100"
// quantity.FinalCalculationSteps -> "Quantity = 3"
```

### 2. Arithmetic operators with automatic precedence

`+`, `-`, `*`, `/` are overloaded. Multiplication/division have higher precedence than addition/subtraction, and parentheses are added to the rendered caption automatically when needed.

```csharp
var a = 2m.As("a");
var b = 3m.As("b");
var c = 4m.As("c");

var sumThenTimes = (a + b) * c;
var timesThenSum = a * b + c;

// sumThenTimes.FinalCalculationSteps -> "(a[2] + b[3]) × c[4] = 20"
// timesThenSum.FinalCalculationSteps -> "a[2] × b[3] + c[4] = 10"
```

### 3. Naming intermediate results — `.As("NewName")` on a computed value

Wrap a sub-expression with a name and the trace expands into discrete, labeled steps.

```csharp
var basePrice    = 200m.As("BasePrice");
var discountRate = 0.15m.As("DiscountRate");
var taxRate      = 0.08m.As("TaxRate");

var discount         = (basePrice * discountRate).As("Discount");
var discountedPrice  = (basePrice - discount).As("DiscountedPrice");
var tax              = (discountedPrice * taxRate).As("Tax");
var finalTotal       = (discountedPrice + tax).As("FinalTotal");

Console.WriteLine(finalTotal.FinalCalculationSteps);
```

Output:

```
Discount = BasePrice[200] × DiscountRate[0.15] = 30

DiscountedPrice = BasePrice[200] - Discount[30] = 170

Tax = DiscountedPrice[170] × TaxRate[0.08] = 13.6

FinalTotal = DiscountedPrice[170] + Tax[13.6] = 183.6
```

### 4. Capturing variable & property names — `ValueWithCaption.From(expression)`

Build a value from a lambda and the library will reflect on the expression tree to pick up the source variable / property name as the caption. Supports `decimal`, `int`, `double`, `float`.

```csharp
var basePrice = 100m;
var rate      = 0.18m;

var price = ValueWithCaption.From(() => basePrice);
var tax   = ValueWithCaption.From(() => rate);

var taxAmount = price * tax;
// taxAmount.FinalCalculationSteps -> "basePrice[100] × rate[0.18] = 18"
```

Property and field references work the same way:

```csharp
var product = new Product { Price = 99.99m };
var p = ValueWithCaption.From(() => product.Price);
// p.FinalCalculationSteps -> "Price"
```

### 5. `[DisplayName]` for human-friendly property captions

If a property is annotated with `System.ComponentModel.DisplayNameAttribute`, that label is used instead of the raw property name.

```csharp
class Product
{
    [DisplayName("Product Cost")]
    public decimal Cost { get; set; }
}

var product = new Product { Cost = 150m };
var c = ValueWithCaption.From(() => product.Cost);
// c.FinalCalculationSteps -> "Product Cost"
```

### 6. LINQ-style `Sum` over collections

Two overloads, mirroring `Enumerable.Sum`: one for a projection, one for a direct collection of `ValueWithCaption`.

```csharp
var employees = new[]
{
    new { Advance = 500m.As("Employee1Advance") },
    new { Advance = 750m.As("Employee2Advance") },
    new { Advance = 300m.As("Employee3Advance") },
};

var total = employees.Sum(e => e.Advance);
// total.FinalCalculationSteps
//   -> "Employee1Advance[500] + Employee2Advance[750] + Employee3Advance[300] = 1,550"
```

**Expanded vs compact:** with 1–3 items the trace lists each addend; with 4+ items it switches to a compact `Sum(<commonName>, count(N))` form so traces stay readable on large collections.

```csharp
var values = new[]
{
    100m.As("Value1"), 200m.As("Value2"),
    150m.As("Value3"), 250m.As("Value4"),
};

var total = values.Sum();
// total.FinalCalculationSteps -> "Sum(Value, count(4))[700] = 700"
```

Calculation steps from inner expressions are preserved when summing wrapped values:

```csharp
var advance1 = (2000m.As("Salary1") * 0.25m.As("Rate")).As("Advance1");
var advance2 = (3000m.As("Salary2") * 0.30m.As("Rate")).As("Advance2");

var totalAdvances = new[] { advance1, advance2 }.Sum().As("TotalAdvances");
```

Output:

```
Advance1 = Salary1[2,000] × Rate[0.25] = 500

Advance2 = Salary2[3,000] × Rate[0.3] = 900

TotalAdvances = Advance1[500] + Advance2[900] = 1,400
```

### 7. Automatic multi-line formatting for long expressions

When an expression has more than three operators (or exceeds ~150 characters), the trace is broken across lines with aligned operators and indented sub-expressions, so long calculations stay legible.

```csharp
var q1 = new[] { 1000m.As("Jan"), 1200m.As("Feb"),  900m.As("Mar") }.Sum();
var q2 = new[] { 1100m.As("Apr"), 1300m.As("May"), 1000m.As("Jun") }.Sum();
var q3 = new[] {  950m.As("Jul"), 1150m.As("Aug"), 1250m.As("Sep") }.Sum();

var halfYear = q1 + q2 + q3;
```

Output (note the aligned `+` column):

```
  Jan[1,000]
+ Feb[1,200]
+ Mar[900]
+ Apr[1,100]
+ May[1,300]
+ Jun[1,000]
+ Jul[950]
+ Aug[1,150]
+ Sep[1,250]
= 9,850
```

And with mixed operators and parentheses:

```csharp
var groupA = new[] { 10m.As("A1"), 15m.As("A2") }.Sum();
var groupB = new[] { 20m.As("B1"), 25m.As("B2") }.Sum();
var groupC = new[] {  5m.As("C1"), 10m.As("C2") }.Sum();

var result = (groupA + groupB) * groupC - 50m.As("Deduction");
```

Output:

```
  (  A1[10]
   + A2[15]
   + B1[20]
   + B2[25]
  )
×
  (  C1[5]
   + C2[10]
  )
- Deduction[50]
= 1,000
```

### 8. Comparison operators

`>`, `<`, `>=`, `<=`, `==`, `!=` are overloaded — both between two `ValueWithCaption` and between a `ValueWithCaption` and a raw `decimal`. The library also implements `IComparable` and `IComparable<ValueWithCaption>`, so values can be sorted with LINQ's `OrderBy`.

```csharp
var a = 10m.As("a");
var b = 5m.As("b");

bool gt        = a > b;        // true
bool gtDecimal = a > 7.5m;     // true

var sorted = new[] { a, b }.OrderBy(x => x).ToList();
```

Equality is **value-based**: `a == b` is true iff `a.Value == b.Value`, regardless of caption. This keeps `==`, `Equals`, and `CompareTo` in agreement.

### 9. Static helpers — `StaticValues.Zero`, `StaticValues.One`

Convenience constants for use in folds and seeded reductions:

```csharp
var total = items
    .Select(i => i.Amount.As("Amount"))
    .Aggregate(StaticValues.Zero, (acc, x) => acc + x);
```

### 10. Number formatting in output

Every value is rendered through a `NumberFormat`. The default, `NumberFormat.Default`:

- Rounds to **2 decimal places**, midpoints away from zero
- Never shows fewer than **3 significant digits**, so a rate like `0.045` stays `0.045` instead of becoming `0.05`
- Strips trailing zeros after the decimal point
- Inserts **thousand separators** (`,`)
- Drops the decimal point entirely when the value is an integer

| Raw value      | Rendered as |
|----------------|-------------|
| `100m`         | `100`       |
| `1500m`        | `1,500`     |
| `1234.5m`      | `1,234.5`   |
| `1234.56789m`  | `1,234.57`  |
| `0.18m`        | `0.18`      |
| `0.045m`       | `0.045`     |
| `0.0125m`      | `0.0125`    |

For amounts that should line up in a column, pass `NumberFormat.Money` (always two decimals) to `As`. A value keeps its format wherever it is printed, and the result of an operation inherits the format of the first operand that has one, so money times a rate is still money:

```csharp
var jul = 979.1m.As("Jul", NumberFormat.Money);
var jun = 1020m.As("Jun", NumberFormat.Money);
var may = 1063.1m.As("May", NumberFormat.Money);

var total = jul + jun + may;
// Jul[979.10] + Jun[1,020.00] + May[1,063.10] = 3,062.20

var bonus = (1000m.As("Salary", NumberFormat.Money) * 0.05m.As("rate")).As("Bonus");
// Bonus = Salary[1,000.00] × rate[0.05] = 50.00
```

Custom formats are plain records: `new NumberFormat(maxDecimals: 4)`. `value.FormattedValue` returns the number exactly as it appears in the steps.

### 11. Captions can contain anything

Caption text is never interpreted as arithmetic. A caption such as `BB - card amount (gross)` is printed as one term, hyphen and parentheses included, and is never split into a subtraction. Captions containing `=`, brackets or decimal numbers (`ბრუტო 0.000000`) are also left exactly as written.

### 12. Named steps are referred to, not re-expanded

Once a sub-expression has been given a name with `.As()`, any later occurrence of the same expression prints as `Name[value]`, even when the calculation kept using the unnamed copy:

```csharp
var gross = a + b + c + d;
var grossNamed = gross.As("Gross");
var pension = (gross * pensionRate).As("Pension");   // built from the unnamed copy
var net = (grossNamed - pension).As("Net");
```

```
Gross = a[10] + b[20] + c[30] + d[40] = 100

Pension = Gross[100] × pension %[0.02] = 2

Net = Gross[100] - Pension[2] = 98
```

### 13. CRLF-stable output

`FinalCalculationSteps` always emits **CRLF** (`\r\n`) line endings, regardless of host platform, so output is byte-stable across Windows, Linux, and macOS — handy for snapshot tests and reproducible reports.

---

## Use case

The library exists for one job: making a calculation **explain itself** to a non-technical reader. Payroll slips, tax breakdowns, invoice reconciliations, audit reports — anywhere the user looks at a final number and asks *"how did you get this?"*, you can hand them `FinalCalculationSteps`.

## License

See [LICENSE](LICENSE).

*Collaboration by Claude*
