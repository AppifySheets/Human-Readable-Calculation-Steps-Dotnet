using System.Text.RegularExpressions;
using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    // Covers GitHub issue #48: a sub-expression that is used more than once should be
    // derived once and referred to by name afterwards, whether the caller named it with
    // As() or left it unnamed.
    public class NamedSubExpressionTests
    {
        static (ValueWithCaption a, ValueWithCaption b, ValueWithCaption c, ValueWithCaption d) Terms() =>
            (10m.As("a"), 20m.As("b"), 30m.As("c"), 40m.As("d"));

        [Fact]
        public void UnnamedCopyOfANamedExpression_IsPrintedByName()
        {
            var (a, b, c, d) = Terms();
            var pensionRate = 0.02m.As("pension %");
            var taxRate = 0.2m.As("tax %");

            // The caller names the gross once, but keeps computing with the unnamed
            // expression (a typical "recomputed property" pattern).
            var gross = a + b + c + d;
            var grossNamed = gross.As("Gross");
            var pension = (gross * pensionRate).As("Pension");
            var tax = ((gross - pension) * taxRate).As("Tax");

            var net = (grossNamed - pension - tax).As("Net");

            var expected =
"""
Gross = a[10] + b[20] + c[30] + d[40] = 100

Pension = Gross[100] × pension %[0.02] = 2

Tax = 
  (  Gross[100]
   - Pension[2]
  )
× tax %[0.2]
= 19.6

Net = Gross[100] - Pension[2] - Tax[19.6] = 78.4
""";
            Assert.Equal(expected, net.FinalCalculationSteps);
        }

        [Fact]
        public void FinalExpression_RefersToNamedStepsInsteadOfExpanding()
        {
            var (a, b, c, d) = Terms();
            var pensionRate = 0.02m.As("pension %");

            var gross = a + b + c + d;
            var grossNamed = gross.As("Gross");
            var pension = (gross * pensionRate).As("Pension");

            // Unnamed final expression that inlines the gross subtree again.
            var net = grossNamed - pension - gross * 0m.As("zero");

            var expected =
"""
Gross = a[10] + b[20] + c[30] + d[40] = 100

Pension = Gross[100] × pension %[0.02] = 2

Gross[100] - Pension[2] - Gross[100] × zero[0] = 98
""";
            Assert.Equal(expected, net.FinalCalculationSteps);
        }

        [Fact]
        public void UnnamedExpressionUsedTwice_GetsAGeneratedName()
        {
            var (a, b, c, d) = Terms();
            var pensionRate = 0.02m.As("pension %");

            var gross = a + b + c + d;
            var pension = (gross * pensionRate).As("Pension");
            var net = gross - pension;

            // The gross appears twice (inside Pension and in the final line) and was never
            // named, so it is derived once under a generated name.
            var expected =
"""
#1 = a[10] + b[20] + c[30] + d[40] = 100

Pension = #1[100] × pension %[0.02] = 2

#1[100] - Pension[2] = 98
""";
            Assert.Equal(expected, net.FinalCalculationSteps);
        }

        [Fact]
        public void UnnamedSharedComposite_IsDerivedOnce()
        {
            // Reproduction from the issue: identical arithmetic to the named case below, but
            // the shared part was never given a name.
            var unnamed = 10m.As("A") + 20m.As("B");

            var total = ((unnamed + 1m.As("C")) + (unnamed + 2m.As("D"))).As("Total");

            var expected =
"""
#1 = A[10] + B[20] = 30

Total = #1[30] + C[1] + #1[30] + D[2] = 63
""";
            Assert.Equal(expected, total.FinalCalculationSteps);
        }

        [Fact]
        public void ExplicitName_WinsOverGeneratedName()
        {
            var shared = (10m.As("A") + 20m.As("B")).As("Shared");

            var total = ((shared + 1m.As("C")) + (shared + 2m.As("D"))).As("Total");

            var expected =
"""
Shared = A[10] + B[20] = 30

Total = Shared[30] + C[1] + Shared[30] + D[2] = 63
""";
            Assert.Equal(expected, total.FinalCalculationSteps);
        }

        [Fact]
        public void GeneratedDefinitions_ComeAfterTheNamedStepsTheyUse()
        {
            var basePay = (100m.As("p") * 0.5m.As("r")).As("Base");
            var shared = basePay + 1m.As("k");

            var total = (shared + shared).As("Total");

            var expected =
"""
Base = p[100] × r[0.5] = 50

#1 = Base[50] + k[1] = 51

Total = #1[51] + #1[51] = 102
""";
            Assert.Equal(expected, total.FinalCalculationSteps);
        }

        [Fact]
        public void DoublingTree_GrowsLinearlyWithDepth()
        {
            // Each level references the previous one twice without naming it. In 1.3.3 this
            // produced 514 lines at depth 8; every level should now be a single definition.
            var current = 1m.As("Leaf") + 0m.As("Zero");
            for (var i = 0; i < 8; i++)
                current = current + current;

            var steps = current.FinalCalculationSteps;
            var lines = steps.Split("\r\n");

            Assert.Equal(256m, current.Value);
            Assert.Single(Regex.Matches(steps, @"Leaf\["));
            Assert.Equal(17, lines.Length);
            Assert.Equal("#1 = Leaf[1] + Zero[0] = 1", lines[0]);
            Assert.Equal("#8[128] + #8[128] = 256", lines[^1]);
        }
    }
}
