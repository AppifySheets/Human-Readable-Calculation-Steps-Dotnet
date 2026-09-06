using HumanReadableCalculationSteps;
using System.Text;

// Generates a Markdown showcase of representative library outputs. Each
// example pairs the C# code that produced a value with the FinalCalculationSteps
// string it yielded — so reviewers reading the CI artifact see both the input
// and the human-readable result without running the code themselves.

var output = new StringBuilder();
output.AppendLine("# HumanReadableCalculationSteps — Output Showcase");
output.AppendLine();
output.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
output.AppendLine();
output.AppendLine("Each section below shows a representative use case: the C# code on the");
output.AppendLine("left, and the `FinalCalculationSteps` string the library produced on the");
output.AppendLine("right. This file is regenerated on every CI run from real library output —");
output.AppendLine("if it changes, the library's formatting changed too.");
output.AppendLine();

// Each example pairs a name, the source code (as a string for display) and a
// factory that produces the actual ValueWithCaption. Keeping these as tuples
// is deliberate — a struct/class would just be noise for a flat list.
var examples = new (string Title, string Code, Func<ValueWithCaption> Build)[]
{
    (
        "Basic arithmetic with precedence",
        "var a = 10m.As(\"a\");\nvar b = 5m.As(\"b\");\nvar c = 3m.As(\"c\");\nvar result = a + b * c;",
        () =>
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 3m.As("c");
            return a + b * c;
        }
    ),
    (
        "Parentheses override precedence",
        "var a = 10m.As(\"a\");\nvar b = 5m.As(\"b\");\nvar c = 3m.As(\"c\");\nvar result = (a + b) * c;",
        () =>
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 3m.As("c");
            return (a + b) * c;
        }
    ),
    (
        "Division with rounding",
        "var total = 100m.As(\"total\");\nvar parts = 3m.As(\"parts\");\nvar each = total / parts;",
        () =>
        {
            var total = 100m.As("total");
            var parts = 3m.As("parts");
            return total / parts;
        }
    ),
    (
        "Wrapped intermediate value (.As())",
        "var basePrice = 100m.As(\"BasePrice\");\nvar discount = 15m.As(\"Discount\");\nvar net = (basePrice - discount).As(\"NetPrice\");\nvar taxRate = 0.18m.As(\"TaxRate\");\nvar total = net + net * taxRate;",
        () =>
        {
            var basePrice = 100m.As("BasePrice");
            var discount = 15m.As("Discount");
            var net = (basePrice - discount).As("NetPrice");
            var taxRate = 0.18m.As("TaxRate");
            return net + net * taxRate;
        }
    ),
    (
        "Multi-level dependency chain",
        "var qty = 15m.As(\"Quantity\");\nvar price = 100m.As(\"UnitPrice\");\nvar subtotal = (qty * price).As(\"Subtotal\");\nvar discountRate = 0.1m.As(\"DiscountRate\");\nvar discount = (subtotal * discountRate).As(\"Discount\");\nvar final = subtotal - discount;",
        () =>
        {
            var qty = 15m.As("Quantity");
            var price = 100m.As("UnitPrice");
            var subtotal = (qty * price).As("Subtotal");
            var discountRate = 0.1m.As("DiscountRate");
            var discount = (subtotal * discountRate).As("Discount");
            return subtotal - discount;
        }
    ),
    (
        "LINQ Sum across a collection",
        "var monthly = new[] {\n    1000m.As(\"Jan\"),\n    1100m.As(\"Feb\"),\n    1250m.As(\"Mar\"),\n};\nvar total = monthly.Sum();",
        () =>
        {
            var monthly = new[]
            {
                1000m.As("Jan"),
                1100m.As("Feb"),
                1250m.As("Mar"),
            };
            return monthly.Sum();
        }
    ),
    (
        "Sum result used in further arithmetic",
        "var prices = new[] { 100m.As(\"P1\"), 200m.As(\"P2\") };\nvar subtotal = prices.Sum().As(\"Subtotal\");\nvar taxRate = 0.1m.As(\"TaxRate\");\nvar tax = subtotal * taxRate;",
        () =>
        {
            var prices = new[] { 100m.As("P1"), 200m.As("P2") };
            var subtotal = prices.Sum().As("Subtotal");
            var taxRate = 0.1m.As("TaxRate");
            return subtotal * taxRate;
        }
    ),
    (
        "Long expression — multiline formatting",
        // Showcases the formatter's auto-multiline behavior when an expression
        // gets long enough (>3 operators or >150 chars).
        "var values = new[] { 100m.As(\"a\"), 200m.As(\"b\"), 50m.As(\"c\"), 75m.As(\"d\"), 25m.As(\"e\") };\nvar weight = 1.5m.As(\"weight\");\nvar result = (values[0] + values[1] + values[2] + values[3] + values[4]) * weight;",
        () =>
        {
            var values = new[] { 100m.As("a"), 200m.As("b"), 50m.As("c"), 75m.As("d"), 25m.As("e") };
            var weight = 1.5m.As("weight");
            return (values[0] + values[1] + values[2] + values[3] + values[4]) * weight;
        }
    ),
    (
        "Unicode (Georgian script)",
        // Real-world usage — the library was originally built for Georgian
        // financial calculation reports.
        "var საბაზო = 100m.As(\"საბაზო ფასი\");\nvar ფასდაკლება = 15m.As(\"ფასდაკლება\");\nvar დღგ = 0.18m.As(\"დღგ\");\nvar one = 1m.As(\"1\");\nvar result = (საბაზო - ფასდაკლება) * (one + დღგ);",
        () =>
        {
            var საბაზო = 100m.As("საბაზო ფასი");
            var ფასდაკლება = 15m.As("ფასდაკლება");
            var დღგ = 0.18m.As("დღგ");
            var one = 1m.As("1");
            return (საბაზო - ფასდაკლება) * (one + დღგ);
        }
    ),
};

foreach (var (title, code, build) in examples)
{
    var value = build();
    var steps = value.FinalCalculationSteps;
    var resultValue = value.FormattedValue;

    output.AppendLine($"## {title}");
    output.AppendLine();
    output.AppendLine("**Code:**");
    output.AppendLine();
    output.AppendLine("```csharp");
    output.AppendLine(code);
    output.AppendLine("```");
    output.AppendLine();
    output.AppendLine($"**Numeric result:** `{resultValue}`");
    output.AppendLine();
    output.AppendLine("**FinalCalculationSteps:**");
    output.AppendLine();
    output.AppendLine("```");
    output.AppendLine(steps);
    output.AppendLine("```");
    output.AppendLine();
}

// Path is provided via the first CLI arg so CI can target an artifact dir.
// Default to the working directory for ad-hoc local runs.
var targetPath = args.Length > 0 ? args[0] : "examples.md";
File.WriteAllText(targetPath, output.ToString());

Console.WriteLine($"Wrote {examples.Length} examples to {Path.GetFullPath(targetPath)}");

// Also dump to stdout so the workflow log shows the report inline without
// needing to download the artifact for quick eyeballing.
Console.WriteLine();
Console.WriteLine(output.ToString());
