using MathService.Services;

namespace MathService.Endpoints;

public static class DateEndpoints
{
    public static void MapDateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/date");

        group.MapGet("/daysbetween", (DateTime from, DateTime to)
            => Results.Ok(new { days = DateService.DaysBetween(from, to) }));

        group.MapGet("/isweekend", (DateTime date)
            => Results.Ok(new { date, isWeekend = DateService.IsWeekend(date) }));

        group.MapGet("/age", (DateTime birthDate)
            => Results.Ok(new { age = DateService.GetAge(birthDate) }));

        group.MapGet("/dayname", (DateTime date)
            => Results.Ok(new { day = DateService.GetDayName(date) }));
    }
}