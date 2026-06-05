namespace MathService.Services;

public static class DateService
{
    public static int DaysBetween(DateTime from, DateTime to)
        => Math.Abs((to - from).Days);

    public static bool IsWeekend(DateTime date)
        => date.DayOfWeek == DayOfWeek.Saturday
        || date.DayOfWeek == DayOfWeek.Sunday;

    public static int GetAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    public static string GetDayName(DateTime date)
        => date.DayOfWeek.ToString();
}