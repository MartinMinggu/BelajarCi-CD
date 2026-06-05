using MathService.Services;

namespace MathService.Endpoints;

public static class MathEndpoints
{
    public static void MapMathEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/math");
        group.MapGet("/", () => "MathService v3  Railway is running");
        group.MapGet("/add", (int a, int b) => new { result = MathLogic.Add(a, b) });
        group.MapGet("/multiply", (int a, int b) => new { result = MathLogic.Multiply(a, b) });
        group.MapGet("/kurang", (int a, int b) => new { result = MathLogic.Subtract(a, b) });
        group.MapGet("/bagi", (int a, int b) => new { result = MathLogic.Divide(a, b) });
        group.MapGet("/mod", (int a, int b) => new { result = MathLogic.Mod(a, b) });
        group.MapGet("/cek_ganjil_genap", (int a) => new { result = "Bilangan " + a + " adalah " + MathLogic.GanjilGenap(a) });
    }
}
