namespace MathService.Services;

public static class MathLogic
{
    public static int Add(int a, int b) => a + b;
    public static int Multiply(int a, int b) => a * b;

    public static int Subtract(int a, int b) => a - b;
    public static int Mod(int a, int b) => a % b;
    public static string GanjilGenap(int a) => a % 2 == 0 ? "Genap" : "Ganjil";
    public static double Divide(double a, double b)
    {
        if (b == 0) throw new DivideByZeroException("Tidak bisa di agi dengan 0");
        return a / b;
    }
     public static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i <= Math.Sqrt(n); i++)
            if (n % i == 0) return false;
        return true;
    }
}