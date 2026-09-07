using System.Globalization;
using System.Text.RegularExpressions;
using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    // Verifies that additive sub-expressions are bracketed when they sit inside a
    // multiplication or division (and on the right of a subtraction or division), and
    // that they are NOT bracketed where precedence already reads correctly.
    //
    // The second half re-evaluates the printed text as arithmetic and compares it with
    // the value the library computed. A missing or misplaced bracket changes what the
    // text says the answer should be, which is the failure class behind issue #47.
    public class ParenthesisationTests
    {
        static ValueWithCaption A => 10m.As("a");
        static ValueWithCaption B => 20m.As("b");
        static ValueWithCaption C => 4m.As("c");
        static ValueWithCaption D => 5m.As("d");

        [Fact]
        public void AdditiveOperand_InsideMultiplicationOrDivision_IsBracketed()
        {
            Assert.Equal("(a[10] + b[20]) × c[4] = 120", ((A + B) * C).FinalCalculationSteps);
            Assert.Equal("c[4] × (a[10] + b[20]) = 120", (C * (A + B)).FinalCalculationSteps);
            Assert.Equal("(a[10] - b[20]) × c[4] = -40", ((A - B) * C).FinalCalculationSteps);
            Assert.Equal("c[4] × (a[10] - b[20]) = -40", (C * (A - B)).FinalCalculationSteps);
            Assert.Equal("(a[10] + b[20]) / c[4] = 7.5", ((A + B) / C).FinalCalculationSteps);
            Assert.Equal("c[4] / (a[10] + b[20]) = 0.133", (C / (A + B)).FinalCalculationSteps);
            Assert.Equal("(a[10] - b[20]) / c[4] = -2.5", ((A - B) / C).FinalCalculationSteps);
        }

        [Fact]
        public void MultiplicativeOperand_InsideAdditionOrSubtraction_IsNotBracketed()
        {
            // × and / already bind tighter, so brackets would only add noise.
            Assert.Equal("a[10] + b[20] × c[4] = 90", (A + B * C).FinalCalculationSteps);
            Assert.Equal("a[10] - b[20] × c[4] = -70", (A - B * C).FinalCalculationSteps);
            Assert.Equal("a[10] + b[20] / c[4] = 15", (A + B / C).FinalCalculationSteps);
            Assert.Equal("a[10] - b[20] / c[4] = 5", (A - B / C).FinalCalculationSteps);
            Assert.Equal("b[20] × c[4] + a[10] = 90", (B * C + A).FinalCalculationSteps);
            Assert.Equal("b[20] / c[4] - a[10] = -5", (B / C - A).FinalCalculationSteps);
        }

        [Fact]
        public void RightOperand_OfNonAssociativeOperator_IsBracketed()
        {
            // a - (b - c) and a / (b / c) mean something different without the brackets.
            Assert.Equal("a[10] - (b[20] - c[4]) = -6", (A - (B - C)).FinalCalculationSteps);
            Assert.Equal("a[10] - (b[20] + c[4]) = -14", (A - (B + C)).FinalCalculationSteps);
            Assert.Equal("b[20] / (c[4] / d[5]) = 25", (B / (C / D)).FinalCalculationSteps);
            Assert.Equal("b[20] / (c[4] × d[5]) = 1", (B / (C * D)).FinalCalculationSteps);

            // Left-associative groupings need none: a + (b - c) is a + b - c.
            Assert.Equal("a[10] + b[20] - c[4] = 26", (A + (B - C)).FinalCalculationSteps);
            Assert.Equal("b[20] × c[4] / d[5] = 16", (B * (C / D)).FinalCalculationSteps);
        }

        [Fact]
        public void NamedAdditiveExpression_NeedsNoBrackets_WhenReferredToByName()
        {
            var sum = (A + B).As("Sum");

            // The reference is a single term, so brackets would be wrong here.
            Assert.Equal("Sum = a[10] + b[20] = 30\r\n\r\nSum[30] × c[4] = 120", (sum * C).FinalCalculationSteps);
        }

        [Fact]
        public void LinqSum_IsBracketed_WhereItsAdditiveNatureWouldBeLost()
        {
            var items = new[] { 10m.As("x"), 20m.As("y") };

            Assert.Equal("(x[10] + y[20]) × c[4] = 120", (items.Sum() * C).FinalCalculationSteps);
            Assert.Equal("a[10] - (x[10] + y[20]) = -20", (A - items.Sum()).FinalCalculationSteps);
            Assert.Equal("a[10] + x[10] + y[20] = 40", (A + items.Sum()).FinalCalculationSteps);
        }

        public static TheoryData<string, decimal> Expressions()
        {
            var a = 10m.As("a");
            var b = 20m.As("b");
            var c = 4m.As("c");
            var d = 5m.As("d");

            var cases = new List<ValueWithCaption>
            {
                (a + b) * c,
                c * (a + b),
                (a - b) * c,
                c * (a - b),
                (a + b) / c,
                (a - b) / c,
                a + b * c,
                a - b * c,
                a + b / c,
                a - b / c,
                b * c + a,
                b / c - a,
                a - (b - c),
                a - (b + c),
                a + (b - c),
                b / (c / d),
                b / (c * d),
                b * (c / d),
                (a + b) * (c + d),
                (a - b) * (c - d),
                (a + b) / (c + d),
                (a + b) * c - d,
                a - (b + c) * d,
                a - (b + c) / d,
                (a + b - c) * d,
                a * b - c * d,
                (a * b - c) * d,
                a / (b - c) * d,
                ((a + b) * c - d) / c,
                a - (b - (c - d)),
            };

            var data = new TheoryData<string, decimal>();
            foreach (var expression in cases)
                data.Add(expression.FinalCalculationSteps, expression.Value);

            return data;
        }

        [Theory]
        [MemberData(nameof(Expressions))]
        public void PrintedExpression_EvaluatesToTheComputedValue(string steps, decimal expected)
        {
            // Everything before the final "=" is the expression the reader is shown.
            var printed = steps[..steps.LastIndexOf('=')];

            Assert.Equal(expected, Evaluate(printed));
        }

        // Reads back the printed expression: captions carry their value in brackets, so
        // "a[10] + b[20]" becomes "10 + 20", which is then evaluated with the ordinary
        // precedence rules a human reader would apply.
        static decimal Evaluate(string printed)
        {
            var numbersOnly = Regex.Replace(printed, @"[^\s()+\-×/]*\[(-?[\d,.]+)\]", m => m.Groups[1].Value.Replace(",", ""));
            var tokens = Regex.Matches(numbersOnly, @"\d+(?:\.\d+)?|[()+\-×/]").Select(m => m.Value).ToList();
            var position = 0;

            decimal Primary()
            {
                var negate = false;
                if (tokens[position] == "-") { negate = true; position++; }

                decimal value;
                if (tokens[position] == "(")
                {
                    position++;
                    value = Additive();
                    position++; // closing bracket
                }
                else
                {
                    value = decimal.Parse(tokens[position++], CultureInfo.InvariantCulture);
                }

                return negate ? -value : value;
            }

            decimal Multiplicative()
            {
                var value = Primary();
                while (position < tokens.Count && (tokens[position] == "×" || tokens[position] == "/"))
                    value = tokens[position++] == "×" ? value * Primary() : value / Primary();

                return value;
            }

            decimal Additive()
            {
                var value = Multiplicative();
                while (position < tokens.Count && (tokens[position] == "+" || tokens[position] == "-"))
                    value = tokens[position++] == "+" ? value + Multiplicative() : value - Multiplicative();

                return value;
            }

            var result = Additive();
            Assert.Equal(tokens.Count, position); // the whole expression was consumed
            return result;
        }
    }
}
