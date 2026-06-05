using MathService.Services;

public class DateServiceTests
{
    [Fact]
    public void DaysBetween_ReturnsCorrectDays()
    {
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 1, 11);
        Assert.Equal(10, DateService.DaysBetween(from, to));
    }

    [Theory]
    [InlineData(2024, 1, 6, true)]   // Saturday
    [InlineData(2024, 1, 7, true)]   // Sunday
    [InlineData(2024, 1, 8, false)]  // Monday
    public void IsWeekend_ReturnsCorrectResult(int y, int m, int d, bool expected)
        => Assert.Equal(expected, DateService.IsWeekend(new DateTime(y, m, d)));

    [Fact]
    public void GetDayName_ReturnsCorrectName()
        => Assert.Equal("Monday", DateService.GetDayName(new DateTime(2024, 1, 8)));
}