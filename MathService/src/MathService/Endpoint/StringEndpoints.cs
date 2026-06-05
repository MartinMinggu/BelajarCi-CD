using MathService.Services;

namespace MathService.Endpoints;

public static class StringEndpoints
{
    public static void MapStringEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/string");

        group.MapGet("/reverse", (string input)
            => Results.Ok(new { result = StringService.Reverse(input) }));

        group.MapGet("/palindrome", (string input)
            => Results.Ok(new { input, isPalindrome = StringService.IsPalindrome(input) }));

        group.MapGet("/wordcount", (string input)
            => Results.Ok(new { input, wordCount = StringService.WordCount(input) }));

        group.MapGet("/titlecase", (string input)
            => Results.Ok(new { result = StringService.ToTitleCase(input) }));
    }
}