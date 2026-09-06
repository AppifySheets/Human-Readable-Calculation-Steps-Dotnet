namespace HumanReadableCalculationSteps;

/// <summary>
/// Replaces sub-expressions that have already been emitted as a named step with a
/// reference to that name, so a shared value is derived once and referred to by name
/// afterwards (GitHub issue #48).
/// </summary>
/// <remarks>
/// Works on the internal (escaped) expression strings, where the only parentheses and
/// operators present are structural. A textual match is only collapsed when it is a
/// complete operand of the surrounding expression, judged by what precedes and follows
/// it, so <c>a + b</c> inside <c>k × a + b</c> is left alone.
/// </remarks>
static class NamedExpressionCollapser
{
    /// <summary>A wrapped step of the form <c>Name = Expression = Value</c>.</summary>
    internal sealed record Definition(string Name, string Expression, string Value)
    {
        public string Reference => $"{Name}[{Value}]";
    }

    const string StepSeparator = " = ";
    static readonly string[] AdditiveOperators = [" + ", " - "];
    static readonly string[] AllOperators = [" + ", " - ", " × ", " ÷ "];

    /// <summary>Collects the collapsible definitions from a list of internal steps.</summary>
    public static IReadOnlyList<Definition> DefinitionsFrom(IEnumerable<string> steps) =>
        steps
            .Where(VExtensions.IsWrappedValueDefinition)
            .Select(step => step.Split(StepSeparator))
            .Where(parts => parts.Length == 3)
            .Select(parts => new Definition(parts[0].Trim(), parts[1].Trim(), parts[2].Trim()))
            .Where(definition => HasTopLevelOperator(definition.Expression, AllOperators))
            .ToList();

    /// <summary>
    /// Rewrites the expression part of a <c>Name = Expression = Value</c> step; other
    /// step shapes are returned unchanged.
    /// </summary>
    public static string CollapseStep(string step, IReadOnlyList<Definition> definitions)
    {
        var parts = step.Split(StepSeparator);
        return parts.Length == 3
            ? $"{parts[0]}{StepSeparator}{Collapse(parts[1], definitions, exceptName: parts[0].Trim())}{StepSeparator}{parts[2]}"
            : step;
    }

    /// <summary>
    /// Replaces every occurrence of a definition's expression inside
    /// <paramref name="expression"/> with <c>Name[Value]</c>. Longer definitions are
    /// applied first so an outer expression wins over the sub-expressions it contains.
    /// </summary>
    /// <param name="exceptName">A definition to skip, so a step never collapses to itself.</param>
    public static string Collapse(string expression, IReadOnlyList<Definition> definitions, string? exceptName = null) =>
        definitions
            .Where(definition => definition.Name != exceptName)
            .OrderByDescending(definition => definition.Expression.Length)
            .Aggregate(expression, ReplaceOperand);

    static string ReplaceOperand(string expression, Definition definition)
    {
        var target = definition.Expression;
        var isAdditive = HasTopLevelOperator(target, AdditiveOperators);
        var result = new System.Text.StringBuilder();
        var position = 0;

        while (true)
        {
            var index = expression.IndexOf(target, position, StringComparison.Ordinal);
            if (index < 0) break;

            var end = index + target.Length;
            var wrappedInParentheses = index > 0 && end < expression.Length && expression[index - 1] == '(' && expression[end] == ')';

            if (wrappedInParentheses)
            {
                // "(a + b)" as a whole is the operand: replace brackets and content together.
                result.Append(expression, position, index - 1 - position).Append(definition.Reference);
                position = end + 1;
            }
            else if (IsOperandBoundaryBefore(expression, index, isAdditive) && IsOperandBoundaryAfter(expression, end, isAdditive))
            {
                result.Append(expression, position, index - position).Append(definition.Reference);
                position = end;
            }
            else
            {
                // Textual match that is not a sub-tree here; keep it and move on.
                result.Append(expression, position, end - position);
                position = end;
            }
        }

        return result.Append(expression, position, expression.Length - position).ToString();
    }

    // An additive expression (top-level + or -) is a complete operand only after the
    // start, an opening bracket or a "+" (left-associativity makes "c + a + b" safe to
    // read as "c + (a + b)", but "c - a + b" is not "c - (a + b)"). A multiplicative
    // expression additionally tolerates a preceding "-" or "×", but not "÷".
    static bool IsOperandBoundaryBefore(string text, int index, bool isAdditive) =>
        index == 0
        || text[index - 1] == '('
        || EndsWithAt(text, index, " + ")
        || (!isAdditive && (EndsWithAt(text, index, " - ") || EndsWithAt(text, index, " × ")));

    // After the match: end of text, a closing bracket, or an operator that binds no
    // tighter than the expression itself.
    static bool IsOperandBoundaryAfter(string text, int end, bool isAdditive) =>
        end == text.Length
        || text[end] == ')'
        || StartsWithAt(text, end, " + ")
        || StartsWithAt(text, end, " - ")
        || (!isAdditive && (StartsWithAt(text, end, " × ") || StartsWithAt(text, end, " ÷ ")));

    static bool EndsWithAt(string text, int index, string token) =>
        index >= token.Length && string.CompareOrdinal(text, index - token.Length, token, 0, token.Length) == 0;

    static bool StartsWithAt(string text, int index, string token) =>
        index + token.Length <= text.Length && string.CompareOrdinal(text, index, token, 0, token.Length) == 0;

    // True when one of the operators occurs outside any parentheses.
    static bool HasTopLevelOperator(string expression, string[] operators)
    {
        var depth = 0;
        for (var i = 0; i < expression.Length; i++)
        {
            switch (expression[i])
            {
                case '(': depth++; break;
                case ')': depth--; break;
                default:
                    if (depth == 0 && operators.Any(op => StartsWithAt(expression, i, op))) return true;
                    break;
            }
        }

        return false;
    }
}
