namespace HumanReadableCalculationSteps;

/// <summary>
/// Keeps user-supplied caption text from being mistaken for arithmetic by the
/// string-based formatter.
/// </summary>
/// <remarks>
/// The formatter works on strings such as <c>a[10] + BB - card (gross)[0]</c>, where
/// the caption <c>BB - card (gross)</c> contains a " - " and parentheses that look
/// exactly like an operator and a bracket group (GitHub issue #47). Every character the
/// formatter treats as syntax is therefore mapped onto a code point in the Unicode
/// Private Use Area when a caption enters the library, and mapped back when text leaves
/// through <c>FinalCalculationSteps</c>, <c>ToString()</c> or <c>CalculationSteps</c>.
/// The mapping is one-to-one per character, so escaped text keeps its length and every
/// length-based layout heuristic behaves exactly as before.
/// </remarks>
static class CaptionEscaping
{
    static readonly IReadOnlyDictionary<char, char> Forward = new Dictionary<char, char>
    {
        ['('] = '',
        [')'] = '',
        ['['] = '',
        [']'] = '',
        ['+'] = '',
        ['-'] = '',
        ['×'] = '',
        ['÷'] = '',
        ['='] = '',
        // A dot inside a caption ("ბრუტო 0.000000") used to be rewritten by the
        // decimal clean-up regex, which turned it into "ბრუტო 0".
        ['.'] = '',
    };

    static readonly IReadOnlyDictionary<char, char> Backward = Forward.ToDictionary(pair => pair.Value, pair => pair.Key);

    /// <summary>Replaces syntax characters in a caption with their private-use stand-ins.</summary>
    public static string Escape(string caption) => Map(caption, Forward);

    /// <summary>Restores the original characters in text that is about to leave the library.</summary>
    public static string Unescape(string text) => Map(text, Backward);

    static string Map(string text, IReadOnlyDictionary<char, char> map) =>
        text.Any(map.ContainsKey)
            ? new string(text.Select(c => map.TryGetValue(c, out var mapped) ? mapped : c).ToArray())
            : text;
}
