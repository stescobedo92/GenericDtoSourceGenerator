using System.Text;

namespace GenericDto.Analyzers.Extensions;

internal static class StringExtensions
{
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var words = input.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                {
                    result.Append(word.Substring(1).ToLower());
                }
            }
        }

        return result.ToString();
    }

    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var pascalCase = input.ToPascalCase();
        return char.ToLower(pascalCase[0]) + pascalCase.Substring(1);
    }

    public static string EnsureEndsWith(this string input, string suffix) => input.EndsWith(suffix) ? input : input + suffix;

    public static string EnsureStartsWith(this string input, string prefix) => input.StartsWith(prefix) ? input : prefix + input;
}
