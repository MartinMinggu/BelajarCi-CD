using MathService.Services;

public class StringServiceTests
{
    [Fact]
    public void Reverse_NormalString_ReturnsReversed()
        => Assert.Equal("olleh", StringService.Reverse("hello"));

    [Fact]
    public void Reverse_EmptyString_ReturnsEmpty()
        => Assert.Equal("", StringService.Reverse(""));

    [Theory]
    [InlineData("racecar", true)]
    [InlineData("madam", true)]
    [InlineData("hello", false)]
    [InlineData("", true)]
    public void IsPalindrome_ReturnsCorrectResult(string input, bool expected)
        => Assert.Equal(expected, StringService.IsPalindrome(input));

    [Theory]
    [InlineData("hello world", 2)]
    [InlineData("one", 1)]
    [InlineData("  ", 0)]
    public void WordCount_ReturnsCorrectCount(string input, int expected)
        => Assert.Equal(expected, StringService.WordCount(input));

    [Fact]
    public void ToTitleCase_ConvertsCorrectly()
        => Assert.Equal("Hello World", StringService.ToTitleCase("hello world"));
}