using System.Globalization;

namespace HumanReadableCalculationSteps;

/// <summary>
/// Controls how a numeric value is rendered inside calculation steps.
/// </summary>
/// <remarks>
/// A value carries its format wherever it is printed (as an operand, as a wrapped
/// definition, as a result). Results of arithmetic inherit the format of the first
/// operand that declares one, so money × rate stays money. Values with no explicit
/// format use <see cref="Default"/>.
/// </remarks>
public sealed record NumberFormat
{
    /// <summary>
    /// Two decimal places with trailing zeros stripped and thousands separators, but
    /// never fewer than three significant digits, so a rate such as 0.045 is shown as
    /// entered instead of being rounded to 0.05. Integers print without a decimal point.
    /// </summary>
    public static readonly NumberFormat Default = new(maxDecimals: 2, minDecimals: 0, minSignificantDigits: 3);

    /// <summary>
    /// Always exactly two decimal places (1,020.00), for currency amounts that should
    /// line up in a column.
    /// </summary>
    public static readonly NumberFormat Money = new(maxDecimals: 2, minDecimals: 2, minSignificantDigits: 0);

    const int DecimalPrecisionLimit = 28;

    /// <param name="maxDecimals">Decimal places a value is normally rounded to.</param>
    /// <param name="minDecimals">Decimal places always shown, even when they are zeros.</param>
    /// <param name="minSignificantDigits">
    /// When a value is so small that <paramref name="maxDecimals"/> would hide its
    /// leading digits, more decimals are shown until this many significant digits are
    /// visible. Zero disables the rule.
    /// </param>
    public NumberFormat(int maxDecimals, int minDecimals = 0, int minSignificantDigits = 0)
    {
        if (maxDecimals is < 0 or > DecimalPrecisionLimit)
            throw new ArgumentOutOfRangeException(nameof(maxDecimals), maxDecimals, $"Must be between 0 and {DecimalPrecisionLimit}.");
        if (minDecimals < 0 || minDecimals > maxDecimals)
            throw new ArgumentOutOfRangeException(nameof(minDecimals), minDecimals, "Must be between 0 and maxDecimals.");
        if (minSignificantDigits < 0)
            throw new ArgumentOutOfRangeException(nameof(minSignificantDigits), minSignificantDigits, "Must not be negative.");

        MaxDecimals = maxDecimals;
        MinDecimals = minDecimals;
        MinSignificantDigits = minSignificantDigits;
    }

    public int MaxDecimals { get; }
    public int MinDecimals { get; }
    public int MinSignificantDigits { get; }

    /// <summary>
    /// Renders <paramref name="value"/> with invariant-culture digits, a comma as the
    /// thousands separator and a dot as the decimal separator.
    /// </summary>
    public string Format(decimal value)
    {
        var decimals = Math.Min(DecimalPrecisionLimit, Math.Max(MaxDecimals, DecimalsNeededForSignificantDigits(value)));

        // Midpoints round away from zero, which is what a reader checking the arithmetic
        // by hand expects (2.345 -> 2.35). decimal.Round defaults to banker's rounding.
        var rounded = Math.Round(value, decimals, MidpointRounding.AwayFromZero);

        // Normalise a value that rounded to zero so no "-0" sign survives.
        if (rounded == 0m) rounded = 0m;

        var pattern = decimals == 0 ? "#,##0" : "#,##0." + new string('0', decimals);
        var text = rounded.ToString(pattern, CultureInfo.InvariantCulture);

        return TrimTrailingZeros(text);
    }

    // How many decimals are needed so that MinSignificantDigits digits of the value are
    // visible. Only matters for values whose leading digit sits to the right of the
    // MaxDecimals position (0.045 needs four decimals to show "0.045" with three
    // significant digits; 3.333 needs none beyond the default two).
    int DecimalsNeededForSignificantDigits(decimal value)
    {
        if (MinSignificantDigits == 0 || value == 0m) return 0;

        var magnitude = Math.Abs(value);

        if (magnitude >= 1m)
        {
            var integerDigits = 1;
            for (var m = magnitude; m >= 10m; m /= 10m) integerDigits++;
            return Math.Max(0, MinSignificantDigits - integerDigits);
        }

        // Position (1-based, counting decimals) of the first non-zero digit.
        var firstSignificantPosition = 0;
        for (var m = magnitude; m < 1m; m *= 10m) firstSignificantPosition++;
        return firstSignificantPosition - 1 + MinSignificantDigits;
    }

    // Removes trailing zeros after the decimal point down to MinDecimals, and the
    // point itself when nothing is left after it.
    string TrimTrailingZeros(string text)
    {
        var dot = text.IndexOf('.');
        if (dot < 0) return text;

        var keepUntil = dot + 1 + MinDecimals;
        var end = text.Length;
        while (end > keepUntil && text[end - 1] == '0') end--;
        if (end == dot + 1) end = dot;

        return text[..end];
    }
}
