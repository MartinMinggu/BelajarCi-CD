public  partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        var summaries = new[]
        {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        });

        app.MapGet("/", () => "MathService v3  Railway is running");
        app.MapGet("/add", (int a, int b) => new { result = MathLogic.Add(a, b) });
        app.MapGet("/multiply", (int a, int b) => new { result = MathLogic.Multiply(a, b) });
        app.MapGet("/kurang", (int a, int b) => new { result = MathLogic.Kurang(a, b) });
        app.MapGet("/bagi", (int a, int b) => new { result = MathLogic.Bagi(a, b) });
        app.MapGet("/mod", (int a, int b) => new { result = MathLogic.Mod(a, b) });
        app.MapGet("/cek_ganjil_genap", (int a) => new { result = "Bilangan " + a + " adalah " + MathLogic.GanjilGenap(a) });

        app.Run();
    }
    
}
// public partial class Program { }

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
