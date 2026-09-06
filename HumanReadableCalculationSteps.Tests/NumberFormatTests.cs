using System.Globalization;
using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    // Covers GitHub issue #46: values with more than two decimals were silently
    // rounded away (0.045 printed as 0.05), and money could not be shown with a
    // fixed number of decimals.
    public class NumberFormatTests
    {
        static decimal D(string s) => decimal.Parse(s, CultureInfo.InvariantCulture);

        [Theory]
        // Two decimals, trailing zeros stripped, thousands separators (unchanged behaviour).
        [InlineData("100", "100")]
        [InlineData("1500", "1,500")]
        [InlineData("1234.5", "1,234.5")]
        [InlineData("1234.56789", "1,234.57")]
        [InlineData("1020.00", "1,020")]
        [InlineData("1063.10", "1,063.1")]
        [InlineData("294.61753471", "294.62")]
        [InlineData("0", "0")]
        [InlineData("0.18", "0.18")]
        [InlineData("3.333333", "3.33")]
        // Small values keep at least three significant digits instead of collapsing.
        [InlineData("0.045", "0.045")]
        [InlineData("0.005", "0.005")]
        [InlineData("0.0125", "0.0125")]
        [InlineData("0.125", "0.125")]
        [InlineData("0.333333", "0.333")]
        [InlineData("0.000123456", "0.000123")]
        // Midpoints round away from zero, as a person doing the arithmetic would.
        [InlineData("2.345", "2.35")]
        [InlineData("-2.345", "-2.35")]
        [InlineData("-0.045", "-0.045")]
        public void Default_RoundsToTwoDecimals_ButKeepsThreeSignificantDigits(string input, string expected) =>
            Assert.Equal(expected, NumberFormat.Default.Format(D(input)));

        [Theory]
        [InlineData("979.12", "979.12")]
        [InlineData("1020", "1,020.00")]
        [InlineData("1063.1", "1,063.10")]
        [InlineData("294.61753471", "294.62")]
        [InlineData("0", "0.00")]
        [InlineData("0.045", "0.05")]
        public void Money_AlwaysShowsTwoDecimals(string input, string expected) =>
            Assert.Equal(expected, NumberFormat.Money.Format(D(input)));

        [Fact]
        public void RateWithThreeDecimals_IsShownExactlyInSteps()
        {
            var principal = 1000m.As("Principal");
            var rate = 0.045m.As("InterestRate");

            var interest = (principal * rate).As("Interest");

            Assert.Equal("Interest = Principal[1,000] × InterestRate[0.045] = 45", interest.FinalCalculationSteps);
        }

        [Fact]
        public void MoneyFormat_AppliesToEveryOccurrenceOfTheValue()
        {
            var jul = 979.1m.As("Jul", NumberFormat.Money);
            var jun = 1020m.As("Jun", NumberFormat.Money);
            var may = 1063.1m.As("May", NumberFormat.Money);

            var total = jul + jun + may;

            // Terms line up with the same number of decimals, and the result inherits
            // the operands' format, so 3062.20 is not stripped to 3,062.2.
            Assert.Equal("Jul[979.10] + Jun[1,020.00] + May[1,063.10] = 3,062.20", total.FinalCalculationSteps);
        }

        [Fact]
        public void ResultFormat_IsInheritedFromTheOperandThatDeclaresOne()
        {
            var salary = 1000m.As("Salary", NumberFormat.Money);
            var rate = 0.05m.As("rate");

            var bonus = (salary * rate).As("Bonus");

            // Salary is money, the rate is not; money × rate is money.
            Assert.Equal("Bonus = Salary[1,000.00] × rate[0.05] = 50.00", bonus.FinalCalculationSteps);
        }

        [Fact]
        public void WrappedValue_CanOverrideFormat()
        {
            var days = 22m.As("Days");
            var rate = 45.5m.As("DailyRate");

            var pay = (days * rate).As("Pay", NumberFormat.Money);
            var doubled = pay + pay;

            Assert.Equal("Pay = Days[22] × DailyRate[45.5] = 1,001.00", pay.FinalCalculationSteps);
            Assert.Equal("Pay = Days[22] × DailyRate[45.5] = 1,001.00\r\n\r\nPay[1,001.00] + Pay[1,001.00] = 2,002.00", doubled.FinalCalculationSteps);
        }

        [Fact]
        public void IntegerValues_AreNotGivenDecimals_ByDefault()
        {
            var days = 22.As("Days");
            var hours = 8.As("HoursPerDay");

            var total = days * hours;

            Assert.Equal("Days[22] × HoursPerDay[8] = 176", total.FinalCalculationSteps);
        }

        [Fact]
        public void FormattedValue_ExposesTheRenderedResult()
        {
            var v = (1000m.As("P", NumberFormat.Money) * 0.045m.As("r")).As("I");

            Assert.Equal("45.00", v.FormattedValue);
            Assert.Equal(NumberFormat.Money, v.Format);
        }
    }
}
