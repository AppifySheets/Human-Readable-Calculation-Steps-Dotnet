using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    // Covers GitHub issue #47: caption text that happens to contain operator
    // characters (" - ", parentheses, dots in numbers) was parsed as arithmetic by
    // the string-based formatter, inverting signs and splitting captions across
    // lines. Also covers two related rendering faults found while fixing it: a
    // subtracted or divided sub-expression printed without parentheses, and a
    // single-item Sum printing its value twice.
    public class CaptionEscapingTests
    {
        [Fact]
        public void CaptionWithSpacedHyphen_IsNotTreatedAsSubtraction()
        {
            var basePay = 100m.As("base");
            var card = 0m.As("BB - card amount (gross)");
            var duty = 375.9m.As("salary - on duty (gross)");

            var total = basePay + card + duty;

            Assert.Equal("base[100] + BB - card amount (gross)[0] + salary - on duty (gross)[375.9] = 475.9", total.FinalCalculationSteps);
        }

        [Fact]
        public void CaptionWithSpacedHyphen_StaysOnOneLine_InMultilineOutput()
        {
            var a = 10m.As("a");
            var b = 20m.As("b");
            var c = 30m.As("c");
            var d = 40m.As("d");
            var card = 0m.As("BB - card amount (gross)");
            var duty = 375.9m.As("salary - on duty (gross)");

            var total = a + b + c + d + card + duty;
            var lines = total.FinalCalculationSteps.Split("\r\n");

            Assert.Contains("+ BB - card amount (gross)[0]", lines);
            Assert.DoesNotContain(lines, line => line.StartsWith("- "));
            Assert.DoesNotContain(lines, line => line.TrimStart().StartsWith(")"));
        }

        [Fact]
        public void WrappedName_WithSpacedHyphen_IsRecognisedAsADefinition()
        {
            var a = 10m.As("a");
            var card = 0m.As("BB - card amount (gross)");

            var total = (a + card).As("Total - net");

            Assert.Equal("Total - net = a[10] + BB - card amount (gross)[0] = 10", total.FinalCalculationSteps);
        }

        [Fact]
        public void CaptionWithParenthesesAndLeadingMinus_IsKeptIntact()
        {
            var food = 2141.54m.As("საკვების ხარჯის თანხა (2026-08 (აგვისტო) -019 KFC ნატახტარი)");
            var extra = 0m.As("სხვა");

            var meals = (food + extra).As("კვება");
            var steps = meals.FinalCalculationSteps;

            Assert.Contains("საკვების ხარჯის თანხა (2026-08 (აგვისტო) -019 KFC ნატახტარი)[2,141.54]", steps);
            Assert.DoesNotContain(steps.Split("\r\n"), line => line.TrimStart().StartsWith(")"));
        }

        [Fact]
        public void CaptionWithDecimalNumber_IsNotRewrittenInWrappedStep()
        {
            var gross = 0m.As("ბრუტო 0.000000");
            var one = 1m.As("1");

            var fixedSalary = (gross / one).As("ფიქსირებული ხელფასი");

            Assert.Equal("ფიქსირებული ხელფასი = ბრუტო 0.000000[0] ÷ 1[1] = 0", fixedSalary.FinalCalculationSteps);
        }

        [Fact]
        public void CaptionWithEqualsSign_DoesNotBreakStepParsing()
        {
            var x = 5m.As("x = y");
            var a = 10m.As("a");

            var total = (x + a).As("Total");

            Assert.Equal("Total = x = y[5] + a[10] = 15", total.FinalCalculationSteps);
        }

        [Fact]
        public void PublicCalculationSteps_ContainTheOriginalCaptionText()
        {
            var card = 0m.As("BB - card amount (gross)");
            var a = 10m.As("a");

            var total = (a + card).As("Total");

            Assert.Equal("Total = a[10] + BB - card amount (gross)[0] = 10", Assert.Single(total.CalculationSteps));
            Assert.Equal("Total", total.ToString());
        }

        [Fact]
        public void SubtractedSum_IsParenthesised()
        {
            var a = 10m.As("a");
            var b = 20m.As("b");
            var c = 30m.As("c");

            Assert.Equal("a[10] - (b[20] + c[30]) = -40", (a - (b + c)).FinalCalculationSteps);
            Assert.Equal("a[10] - (b[20] - c[30]) = 20", (a - (b - c)).FinalCalculationSteps);
            Assert.Equal("a[10] + b[20] - c[30] = 0", (a + (b - c)).FinalCalculationSteps);
        }

        [Fact]
        public void DividedProduct_IsParenthesised()
        {
            var a = 60m.As("a");
            var b = 2m.As("b");
            var c = 3m.As("c");

            Assert.Equal("a[60] ÷ (b[2] × c[3]) = 10", (a / (b * c)).FinalCalculationSteps);
            Assert.Equal("a[60] ÷ (b[2] ÷ c[3]) = 90", (a / (b / c)).FinalCalculationSteps);
            Assert.Equal("a[60] × b[2] ÷ c[3] = 40", (a * (b / c)).FinalCalculationSteps);
        }

        [Fact]
        public void SubtractedLinqSum_IsParenthesised()
        {
            var revenues = new[] { 1000m.As("Revenue1"), 1500m.As("Revenue2") };
            var expenses = new[] { 300m.As("Expense1"), 200m.As("Expense2") };

            var profit = revenues.Sum() - expenses.Sum();

            Assert.Equal("Revenue1[1,000] + Revenue2[1,500] - (Expense1[300] + Expense2[200]) = 2,000", profit.FinalCalculationSteps);
        }

        [Fact]
        public void SingleItemSum_UsedInArithmetic_ShowsItsValueOnce()
        {
            var single = new[] { 100m.As("Item") }.Sum();
            var other = 5m.As("other");

            Assert.Equal("Item[100]", single.FinalCalculationSteps);
            Assert.Equal("Item[100] - other[5] = 95", (single - other).FinalCalculationSteps);
        }
    }
}
