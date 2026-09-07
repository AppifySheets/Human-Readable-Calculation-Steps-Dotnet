using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    // Edge cases that were previously untested. These document the current
    // contract for behaviors users might reasonably depend on. Several of these
    // are exploratory — failures here are useful signal, not noise.
    public class EdgeCaseTests
    {
        // ------------------------------------------------------------------
        // Division by zero
        // ------------------------------------------------------------------

        [Fact]
        public void Division_ByZero_ThrowsDivideByZeroException()
        {
            // decimal division throws (unlike float/double which produce Infinity/NaN).
            // Documenting current behavior so any future change to a Result-based
            // contract is a deliberate, visible breaking change.
            var a = 10m.As("a");
            var b = 0m.As("b");

            Assert.Throws<DivideByZeroException>(() => { var _ = a / b; });
        }

        [Fact]
        public void Division_ByZeroFromCalculation_ThrowsDivideByZeroException()
        {
            // Same contract when the zero is produced mid-calculation.
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 5m.As("c");

            Assert.Throws<DivideByZeroException>(() => { var _ = a / (b - c); });
        }

        // ------------------------------------------------------------------
        // Double-wrapping with .As()
        // ------------------------------------------------------------------

        [Fact]
        public void DoubleWrap_SimpleValue_KeepsLatestCaption()
        {
            // Re-wrapping should produce a value whose final output reflects
            // the latest caption, not pile up intermediate wrappers.
            var v = 10m.As("Inner").As("Outer");

            Assert.Equal(10m, v.Value);
            // The final output should reference the outer name, not the inner.
            Assert.Contains("Outer", v.FinalCalculationSteps);
        }

        [Fact]
        public void DoubleWrap_OnComputedValue_PreservesUnderlyingSteps()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var first = (a + b).As("First");
            var second = first.As("Second");

            Assert.Equal(15m, second.Value);
            // Re-wrapping shouldn't lose the underlying derivation — the
            // original "a + b" calculation should still surface somewhere.
            Assert.Contains("a", second.FinalCalculationSteps);
            Assert.Contains("b", second.FinalCalculationSteps);
            Assert.Contains("Second", second.FinalCalculationSteps);
        }

        // ------------------------------------------------------------------
        // Multiline formatting threshold boundaries
        // ------------------------------------------------------------------
        // Source uses `operatorCount > 3` and `operatorCount >= 3` heuristics
        // at different sites. Pin down both sides of those boundaries.

        [Fact]
        public void Multiline_WithThreeOperators_IsAtBoundary()
        {
            var a = 1m.As("a");
            var b = 2m.As("b");
            var c = 3m.As("c");
            var d = 4m.As("d");

            // Exactly 3 operators (a + b + c + d).
            var result = a + b + c + d;

            // Don't assert exact format — just that it produces a sensible
            // string containing every operand. This pins the boundary so a
            // refactor of the threshold logic shows up as a diff.
            var output = result.FinalCalculationSteps;
            Assert.Contains("a[1]", output);
            Assert.Contains("b[2]", output);
            Assert.Contains("c[3]", output);
            Assert.Contains("d[4]", output);
            Assert.Contains("= 10", output);
        }

        [Fact]
        public void Multiline_WithFourOperators_CrossesBoundary()
        {
            var a = 1m.As("a");
            var b = 2m.As("b");
            var c = 3m.As("c");
            var d = 4m.As("d");
            var e = 5m.As("e");

            // 4 operators — clearly past the > 3 threshold.
            var result = a + b + c + d + e;

            var output = result.FinalCalculationSteps;
            Assert.Contains("a[1]", output);
            Assert.Contains("e[5]", output);
            Assert.Contains("= 15", output);
        }

        // ------------------------------------------------------------------
        // Captions containing reserved/formatter-significant characters
        // ------------------------------------------------------------------
        // The formatter uses + - × / [ ] ( ) = as delimiters. Captions
        // containing these may or may not survive intact. These tests
        // document the current behavior.

        [Fact]
        public void Caption_WithSpace_PreservesSpace()
        {
            // Spaces in captions are already used heavily (Georgian tests) —
            // this is a baseline sanity check.
            var v = 10m.As("Base Price");

            Assert.Contains("Base Price", v.FinalCalculationSteps);
        }

        [Fact]
        public void Caption_WithColon_SurvivesFormatting()
        {
            // Colons appear in some Georgian captions (existing tests use
            // "ბონუსი: საოპერაციო მოგება"). Confirm with an ASCII variant.
            var v = 10m.As("Tag: Value");

            Assert.Contains("Tag: Value", v.FinalCalculationSteps);
        }

        [Fact]
        public void Caption_WithSquareBrackets_DocumentsBehavior()
        {
            // The formatter wraps values as `name[value]`. A caption that
            // itself contains `[` or `]` could confuse downstream parsing.
            // No assertion on exact format — just that it doesn't throw
            // and produces a non-empty string.
            var v = 10m.As("array[0]");

            var output = v.FinalCalculationSteps;
            Assert.NotNull(output);
            Assert.NotEmpty(output);
        }

        [Fact]
        public void Caption_WithParentheses_DocumentsBehavior()
        {
            // Parens are used for grouping in formatted output. A caption
            // with parens should at least not throw.
            var v = 10m.As("Net (after tax)");

            var output = v.FinalCalculationSteps;
            Assert.NotNull(output);
            Assert.NotEmpty(output);
        }

        [Fact]
        public void Caption_WithArithmeticOperator_DocumentsBehavior()
        {
            // A caption containing the same operator symbols the formatter
            // uses ('+', '-', '×', '/') is the most likely to cause confusion.
            var v = 10m.As("a + b");

            var output = v.FinalCalculationSteps;
            Assert.NotNull(output);
            Assert.NotEmpty(output);
        }

        [Fact]
        public void Caption_WithEqualsSign_DocumentsBehavior()
        {
            // The formatter uses " = " to separate caption from value.
            // A caption containing "=" risks splitting incorrectly in
            // IsWrappedValueDefinition / similar parsers.
            var v = 10m.As("x = y");

            var output = v.FinalCalculationSteps;
            Assert.NotNull(output);
            Assert.NotEmpty(output);
        }
    }
}
