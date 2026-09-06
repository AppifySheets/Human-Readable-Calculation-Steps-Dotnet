using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    // Covers GitHub issue #48: when a sub-expression has already been emitted as a
    // named step, later references to the same expression should print as
    // Name[value] instead of re-expanding the whole subtree.
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
        public void WithoutANamedStep_TheExpressionIsStillExpanded()
        {
            var (a, b, c, d) = Terms();
            var pensionRate = 0.02m.As("pension %");

            var gross = a + b + c + d;
            var pension = (gross * pensionRate).As("Pension");
            var net = gross - pension;

            // Nothing in the output defines the gross, so there is nothing to refer to and
            // the terms are listed in full.
            var steps = net.FinalCalculationSteps;
            Assert.DoesNotContain("Gross", steps);
            Assert.Contains("+ d[40]", steps);
            Assert.Contains("× pension %[0.02]", steps);
        }

        public class CollapserTests
        {
            static readonly NamedExpressionCollapser.Definition Sum = new("S", "a[1] + b[2]", "3");
            static readonly NamedExpressionCollapser.Definition Product = new("P", "a[1] × b[2]", "2");

            [Theory]
            [InlineData("a[1] + b[2]", "S[3]")]
            [InlineData("(a[1] + b[2]) × k[2]", "S[3] × k[2]")]
            [InlineData("k[2] × (a[1] + b[2])", "k[2] × S[3]")]
            [InlineData("a[1] + b[2] + c[3]", "S[3] + c[3]")]
            [InlineData("a[1] + b[2] - c[3]", "S[3] - c[3]")]
            [InlineData("c[3] + a[1] + b[2]", "c[3] + S[3]")]
            // A textual match that is not a sub-tree of the expression must be left alone.
            [InlineData("k[2] × a[1] + b[2]", "k[2] × a[1] + b[2]")]
            [InlineData("a[1] + b[2] × k[2]", "a[1] + b[2] × k[2]")]
            [InlineData("c[3] - a[1] + b[2]", "c[3] - a[1] + b[2]")]
            public void SumDefinition(string expression, string expected) =>
                Assert.Equal(expected, NamedExpressionCollapser.Collapse(expression, [Sum]));

            [Theory]
            [InlineData("a[1] × b[2] ÷ k[2]", "P[2] ÷ k[2]")]
            [InlineData("k[2] - a[1] × b[2]", "k[2] - P[2]")]
            [InlineData("k[2] × a[1] × b[2]", "k[2] × P[2]")]
            [InlineData("k[2] ÷ a[1] × b[2]", "k[2] ÷ a[1] × b[2]")]
            public void ProductDefinition(string expression, string expected) =>
                Assert.Equal(expected, NamedExpressionCollapser.Collapse(expression, [Product]));

            [Fact]
            public void LongerDefinitionsWinOverTheirOwnSubExpressions()
            {
                var outer = new NamedExpressionCollapser.Definition("T", "a[1] + b[2] + c[3]", "6");

                Assert.Equal("T[6] × k[2]", NamedExpressionCollapser.Collapse("(a[1] + b[2] + c[3]) × k[2]", [Sum, outer]));
            }

            [Fact]
            public void ADefinitionDoesNotCollapseItself()
            {
                Assert.Equal("a[1] + b[2]", NamedExpressionCollapser.Collapse("a[1] + b[2]", [Sum], exceptName: "S"));
            }
        }
    }
}
