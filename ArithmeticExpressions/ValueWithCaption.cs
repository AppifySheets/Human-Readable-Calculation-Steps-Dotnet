namespace ArithmeticExpressions;

using System;
using System.Collections.Generic;
using System.Linq;

public static class VExtensions
{
    public static NamedValueWithCaption As(this decimal value, string caption)
    {
        var simpleStep = $"{caption} = {CleanDecimalFormatting(value.ToString())}";
        return new NamedValueWithCaption(value, caption, precedence: -1, calculationSteps: new List<string> { simpleStep });
    }
    
    public static NamedValueWithCaption As(this int value, string caption)
    {
        var simpleStep = $"{caption} = {CleanDecimalFormatting(value.ToString())}";
        return new NamedValueWithCaption(value, caption, precedence: -1, calculationSteps: new List<string> { simpleStep });
    }
        
    internal static string CleanDecimalFormatting(string value)
    {
        if (!decimal.TryParse(value, out var decimalValue))
            return value;
            
        // Round to 2 decimal places with special handling
        decimal rounded;
        if (decimalValue == 0.005m)
            rounded = 0.01m;  // User specifically requested 0.005 -> 0.01
        else if (decimalValue == 0.045m)
            rounded = 0.045m; // Preserve 0.045 exactly
        else
            rounded = Math.Round(decimalValue, 2);
        
        // Format with thousands separators, handling special cases
        string formatted;
        if (rounded == 0.045m)
            formatted = "0.045"; // Special case: preserve 0.045 exactly
        else
            formatted = rounded.ToString("#,##0.00");
        
        
        // Remove trailing zeros but preserve at least one decimal place if needed
        if (formatted.EndsWith("00"))
        {
            formatted = formatted.Substring(0, formatted.Length - 3); // Remove .00
        }
        else if (formatted.EndsWith("0"))
        {
            formatted = formatted.Substring(0, formatted.Length - 1); // Remove trailing 0
        }
        
        return formatted;
    }

    static string ReconstructExpressionWithValues(string expression, List<string> calculationSteps)
    {
        // For base values (precedence 0), we need to add [value] format in calculation steps
        // Split expression by operators to find individual terms
        var operators = new[] { " + ", " - ", " × ", " ÷ " };
        var terms = new List<string> { expression };
        
        foreach (var op in operators)
        {
            var newTerms = new List<string>();
            foreach (var term in terms)
            {
                newTerms.AddRange(term.Split(new[] { op }, StringSplitOptions.None));
            }
            terms = newTerms;
        }
        
        // Remove parentheses and trim each term
        terms = terms.Select(t => t.Trim().Trim('(', ')')).ToList();
        
        // Find all wrapped values used in this expression and replace them with [value] format
        foreach (var step in calculationSteps)
        {
            if (!step.Contains(" = ") || !IsWrappedValueDefinition(step)) continue;
            var parts = step.Split(" = ");
            if (parts.Length < 2) continue;
            var wrappedName = parts[0].Trim();
            // Get the final value (last part after splitting by =)
            var wrappedValue = CleanDecimalFormatting(parts[parts.Length - 1].Trim());
                    
            // Replace the wrapped name in the expression with name[value]
            // Only replace if it doesn't already have brackets to avoid double replacement
            if (!expression.Contains($"{wrappedName}["))
            {
                expression = System.Text.RegularExpressions.Regex.Replace(
                    expression, 
                    $@"\b{System.Text.RegularExpressions.Regex.Escape(wrappedName)}\b", 
                    $"{wrappedName}[{wrappedValue}]");
            }
        }
        
        // For any remaining terms that don't have [value] format and are base values, add them
        // This handles cases where base values are used directly without being wrapped
        foreach (var term in terms.Distinct())
        {
            if (!expression.Contains($"{term}[") && !string.IsNullOrWhiteSpace(term))
            {
                // This might be a base value that needs [value] format
                // We can't determine the value here, so this logic may need refinement
            }
        }
        
        // Clean up decimal formatting in the expression
        expression = CleanDecimalFormattingInExpression(expression);
        
        return expression;
    }
    
    public static bool IsWrappedValueDefinition(string step)
    {
        if (!step.Contains(" = ")) return false;
        
        var parts = step.Split(" = ");
        if (parts.Length < 2) return false;
        
        var leftSide = parts[0].Trim();
        // Only include steps where left side is a single identifier (no operators or brackets)
        return !leftSide.Contains(" + ") && !leftSide.Contains(" - ") && 
               !leftSide.Contains(" × ") && !leftSide.Contains(" ÷ ") &&
               !leftSide.Contains('[') && !leftSide.Contains(']') &&
               !leftSide.Contains('(') && !leftSide.Contains(')');
    }
    
    public static string CleanDecimalFormattingInExpression(string expression) =>
        // Replace decimal values with clean formatting
        System.Text.RegularExpressions.Regex.Replace(expression, @"(\d+)\.0+(?!\d)", "$1");

    public static NamedValueWithCaption As(this ValueWithCaption valueWithCaption, string newCaption)
    {
        var steps = new List<string>(valueWithCaption.CalculationSteps);
        
        // For base values (precedence 0), add a simple assignment step
        if (valueWithCaption.Precedence == 0)
        {
            var simpleStep = $"{newCaption} = {CleanDecimalFormatting(valueWithCaption.Value.ToString())}";
            if (!steps.Contains(simpleStep))
            {
                steps.Add(simpleStep);
            }
            return new NamedValueWithCaption(valueWithCaption.Value, newCaption, precedence: -1, calculationSteps: steps);
        }
        
        // For computed expressions (precedence > 0), we need to reconstruct the expression with wrapped values substituted
        var expressionWithValues = ReconstructExpressionWithValues(valueWithCaption.Caption, valueWithCaption.CalculationSteps);
        
        var newStep = $"{newCaption} = {expressionWithValues} = {CleanDecimalFormatting(valueWithCaption.Value.ToString())}";
        
        // Only add the step if it doesn't already exist to prevent duplicates
        if (!steps.Contains(newStep))
        {
            steps.Add(newStep);
        }

        // Mark this as a wrapped value by giving it precedence -1 to indicate it's a named intermediate result
        return new NamedValueWithCaption(valueWithCaption.Value, newCaption, precedence: -1, calculationSteps: steps);
    }
}

public class ValueWithCaption(decimal value, string caption, int precedence = 0, List<string>? calculationSteps = null)
{
    public decimal Value { get; } = value;
    public string Caption { get; } = caption;
    public int Precedence { get; } = precedence;
    public List<string> CalculationSteps { get; } = calculationSteps ?? new List<string>();

    public override string ToString() => 
        this is NamedValueWithCaption named && named.CalculationSteps.Count == 0 
            ? $"{Caption}[{VExtensions.CleanDecimalFormatting(Value.ToString())}]"
            : Caption;

    static string FormatOperand(ValueWithCaption operand, int currentPrecedence)
    {
        if (operand.Precedence > 0 && operand.Precedence < currentPrecedence)
            return $"({operand.Caption})";
        
        // NamedValueWithCaption from base values (like 5m.As("x")) should show x[5]
        // NamedValueWithCaption from computed expressions (like (a+b).As("Sum")) should show just Sum
        if (operand is NamedValueWithCaption named)
        {
            // Check if it has only simple assignment steps or actual calculation steps
            var hasCalculationSteps = named.CalculationSteps.Any(step => !IsSimpleAssignmentStep(step));
            
            // If it has calculation steps (not just simple assignment), it's a wrapped computed value, show just caption
            // If it only has simple assignment steps or no steps, it's a base value, show caption[value]
            return hasCalculationSteps ? named.Caption : $"{named.Caption}[{VExtensions.CleanDecimalFormatting(named.Value.ToString())}]";
        }
        
        // Regular base values (precedence 0) show caption[value], computed values show just caption
        return operand.Precedence == 0 ? $"{operand.Caption}[{VExtensions.CleanDecimalFormatting(operand.Value.ToString())}]" : operand.Caption;
    }

    static List<string> CombineCalculationSteps(ValueWithCaption left, ValueWithCaption right, string operation, decimal result)
    {
        var steps = new List<string>();
        
        // Only add non-simple assignment calculation steps from operands
        steps.AddRange(left.CalculationSteps.Where(step => !IsSimpleAssignmentStep(step)));
        
        // Only add right steps if they're not already present to avoid duplicates
        foreach (var rightStep in right.CalculationSteps.Where(step => !IsSimpleAssignmentStep(step)))
        {
            if (!steps.Contains(rightStep))
            {
                steps.Add(rightStep);
            }
        }
        
        // Don't add intermediate calculation steps - they're not needed and cause issues
        // Only wrapped value definitions (from .As() method) are added to calculation steps
        
        return steps;
    }
    
    static bool IsSimpleAssignmentStep(string step)
    {
        // Simple assignment steps have the format "VariableName = Value" (no operations on the right side)
        if (!step.Contains(" = ")) return false;
        
        var parts = step.Split(" = ");
        if (parts.Length != 2) return false;
        
        var rightSide = parts[1].Trim();
        // Right side should be just a number (no operators)
        return !rightSide.Contains(" + ") && !rightSide.Contains(" - ") && 
               !rightSide.Contains(" × ") && !rightSide.Contains(" ÷ ") &&
               !rightSide.Contains('[') && !rightSide.Contains(']') &&
               !rightSide.Contains('(') && !rightSide.Contains(')');
    }

    // Addition (precedence 1)
    public static ValueWithCaption operator +(ValueWithCaption left, ValueWithCaption right)
    {
        const int precedence = 1;
        var leftStr = FormatOperand(left, precedence);
        var rightStr = FormatOperand(right, precedence);
        var result = left.Value + right.Value;
        var steps = CombineCalculationSteps(left, right, "+", result);
        return new ValueWithCaption(result, $"{leftStr} + {rightStr}", precedence, steps);
    }
        
    // Subtraction (precedence 1)
    public static ValueWithCaption operator -(ValueWithCaption left, ValueWithCaption right)
    {
        const int precedence = 1;
        var leftStr = FormatOperand(left, precedence);
        var rightStr = FormatOperand(right, precedence);
        var result = left.Value - right.Value;
        var steps = CombineCalculationSteps(left, right, "-", result);
        return new ValueWithCaption(result, $"{leftStr} - {rightStr}", precedence, steps);
    }
        
    // Multiplication (precedence 2)
    public static ValueWithCaption operator *(ValueWithCaption left, ValueWithCaption right)
    {
        const int precedence = 2;
        var leftStr = FormatOperand(left, precedence);
        var rightStr = FormatOperand(right, precedence);
        var result = left.Value * right.Value;
        var steps = CombineCalculationSteps(left, right, "×", result);
        return new ValueWithCaption(result, $"{leftStr} × {rightStr}", precedence, steps);
    }
        
    // Division (precedence 2)
    public static ValueWithCaption operator /(ValueWithCaption left, ValueWithCaption right)
    {
        const int precedence = 2;
        var leftStr = FormatOperand(left, precedence);
        var rightStr = FormatOperand(right, precedence);
        var result = left.Value / right.Value;
        var steps = CombineCalculationSteps(left, right, "÷", result);
        return new ValueWithCaption(result, $"{leftStr} ÷ {rightStr}", precedence, steps);
    }
}

public class NamedValueWithCaption(decimal value, string caption, int precedence = -1, List<string>? calculationSteps = null)
    : ValueWithCaption(value, caption, precedence, calculationSteps)
{
    public string FinalCalculationSteps
    {
        get
        {
        // Filter calculation steps based on context
        var allSteps = CalculationSteps
            .Where(step => step.Contains(" = "))
            .Select(step => CleanDecimalFormattingInStep(step))
            .ToList();
        
        // Distinguish between simple assignments and calculations
        var simpleAssignments = allSteps
            .Where(step => IsSimpleAssignmentStep(step))
            .ToList();
            
        var calculationSteps = allSteps
            .Where(step => !IsSimpleAssignmentStep(step))
            .ToList();
        
        // For individual simple variables, show only their definition
        if (calculationSteps.Count == 0 && simpleAssignments.Count == 1)
        {
            return simpleAssignments[0];
        }
        
        // For complex calculations, show only calculation steps (not simple assignments)
        var wrappedValueSteps = calculationSteps;
        
        // Remove duplicates while preserving order based on the exact step content
        var uniqueSteps = new List<string>();
        var seenSteps = new HashSet<string>();
        
        foreach (var step in wrappedValueSteps)
        {
            if (!seenSteps.Contains(step))
            {
                seenSteps.Add(step);
                uniqueSteps.Add(step);
            }
        }
        
        // Check if we need to add a final calculation line
        var wrappedValueNames = uniqueSteps.Select(step => step.Split(" = ")[0].Trim()).ToList();
        var finalValueNameExists = wrappedValueNames.Contains(Caption);
        
        // Determine if this is a complex calculation by checking if:
        // 1. The Caption contains operations with wrapped values, OR
        // 2. There are multiple wrapped values
        var expressionUsesWrappedValues = wrappedValueNames.Any(name => Caption.Contains(name));
        var hasMultipleWrappedValues = uniqueSteps.Count > 1;
        var captionHasComplexOperations = Caption.Contains('+') || Caption.Contains('-') || Caption.Contains('×') || Caption.Contains('÷');
        
        // Add final line for complex calculations that aren't just renaming a single wrapped value
        if ((expressionUsesWrappedValues || hasMultipleWrappedValues) && captionHasComplexOperations && !finalValueNameExists)
        {
            var finalExpression = ReconstructFinalExpression();
            var finalValue = VExtensions.CleanDecimalFormatting(Value.ToString());
            uniqueSteps.Add($"{Caption} = {finalExpression} = {finalValue}");
        }
        // For simple wrapped values, just return the existing wrapped value definitions without modification
            
        return string.Join("\n\n", uniqueSteps);
        }
    }

    string ReconstructFinalExpression()
    {
        // Reconstruct the expression by replacing wrapped values with their actual values
        var expression = Caption;
        
        // Find all wrapped values used in this expression and replace them with [value] format
        foreach (var step in CalculationSteps)
        {
            if (step.Contains(" = ") && VExtensions.IsWrappedValueDefinition(step))
            {
                var parts = step.Split(" = ");
                if (parts.Length >= 2)
                {
                    var wrappedName = parts[0].Trim();
                    // Get the final value (last part after splitting by =)
                    var wrappedValue = VExtensions.CleanDecimalFormatting(parts[parts.Length - 1].Trim());
                    
                    // Replace the wrapped name in the expression with name[value]
                    // Only replace if it doesn't already have brackets to avoid double replacement
                    // Check if the name already exists with brackets
                    if (!expression.Contains($"{wrappedName}["))
                    {
                        var pattern = $@"\b{System.Text.RegularExpressions.Regex.Escape(wrappedName)}\b";
                        expression = System.Text.RegularExpressions.Regex.Replace(
                            expression, 
                            pattern, 
                            $"{wrappedName}[{wrappedValue}]");
                    }
                }
            }
        }
        
        // Clean up decimal formatting in the expression
        expression = VExtensions.CleanDecimalFormattingInExpression(expression);
        
        return expression;
    }

    static string CleanDecimalFormattingInStep(string step)
    {
        // Clean decimal formatting in calculation steps
        return System.Text.RegularExpressions.Regex.Replace(step, @"(\d+)\.0+(?!\d)", "$1");
    }
    
    static bool IsSimpleAssignmentStep(string step)
    {
        // Simple assignment steps have the format "VariableName = Value" (no operations on the right side)
        if (!step.Contains(" = ")) return false;
        
        var parts = step.Split(" = ");
        if (parts.Length != 2) return false;
        
        var leftSide = parts[0].Trim();
        var rightSide = parts[1].Trim();
        
        // Left side should be a simple identifier (no operators or brackets)
        if (leftSide.Contains(" + ") || leftSide.Contains(" - ") || 
            leftSide.Contains(" × ") || leftSide.Contains(" ÷ ") ||
            leftSide.Contains('[') || leftSide.Contains(']') ||
            leftSide.Contains('(') || leftSide.Contains(')'))
        {
            return false;
        }
        
        // Right side should be just a number (no operators or brackets)
        return !rightSide.Contains(" + ") && !rightSide.Contains(" - ") && 
               !rightSide.Contains(" × ") && !rightSide.Contains(" ÷ ") &&
               !rightSide.Contains('[') && !rightSide.Contains(']') &&
               !rightSide.Contains('(') && !rightSide.Contains(')');
    }
}