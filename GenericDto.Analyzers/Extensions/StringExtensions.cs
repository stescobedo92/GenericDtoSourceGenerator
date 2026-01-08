using System;
using System.Text;

namespace GenericDto.Analyzers.Extensions;

/// <summary>
/// Extension methods for string manipulation.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var words = input.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder(input.Length);

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                {
                    result.Append(word.Substring(1).ToLowerInvariant());
                }
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (input.Length == 1)
            return input.ToLowerInvariant();

        // Check if the string is already camelCase or has special casing
        var pascalCase = input.ToPascalCase();
        if (pascalCase.Length == 0)
            return pascalCase;

        return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
    }

    /// <summary>
    /// Ensures the string ends with the specified suffix.
    /// </summary>
    public static string EnsureEndsWith(this string input, string suffix)
    {
        if (string.IsNullOrEmpty(input))
            return suffix;

        return input.EndsWith(suffix, StringComparison.Ordinal) ? input : input + suffix;
    }

    /// <summary>
    /// Ensures the string starts with the specified prefix.
    /// </summary>
    public static string EnsureStartsWith(this string input, string prefix)
    {
        if (string.IsNullOrEmpty(input))
            return prefix;

        return input.StartsWith(prefix, StringComparison.Ordinal) ? input : prefix + input;
    }

    /// <summary>
    /// Removes the specified suffix from the string if present.
    /// </summary>
    public static string RemoveSuffix(this string input, string suffix)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(suffix))
            return input;

        return input.EndsWith(suffix, StringComparison.Ordinal)
            ? input.Substring(0, input.Length - suffix.Length)
            : input;
    }

    /// <summary>
    /// Removes the specified prefix from the string if present.
    /// </summary>
    public static string RemovePrefix(this string input, string prefix)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(prefix))
            return input;

        return input.StartsWith(prefix, StringComparison.Ordinal)
            ? input.Substring(prefix.Length)
            : input;
    }
}
