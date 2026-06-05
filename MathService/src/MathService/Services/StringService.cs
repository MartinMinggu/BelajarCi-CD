namespace MathService.Services;

public static class StringService
{
    public static string Reverse(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return new string([.. input.Reverse()]);
    }
    public static bool IsPalindrome(string input)
    {
        if (string.IsNullOrEmpty(input)) return true;
        var clean = input.ToLower().Replace(" ", "");
        return clean == Reverse(clean);
    }
    public static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return string.Join(" ", input.Split(' ').Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..].ToLower() : w));
    }
    public static int WordCount(string input)
    {
        if (string.IsNullOrEmpty(input)) return 0;
        return input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}