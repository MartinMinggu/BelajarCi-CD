public static class MathLogic
{
    public static int Add(int a, int b) => a + b;
    public static int Multiply(int a, int b) => a * b;

    public static double Bagi(int a, int b) => a - b;

    public static int Kurang(int a, int b) => a / b;
    public static int Mod(int a, int b) => a % b;
    public static string GanjilGenap(int a) => a % 2 == 0 ? "Genap" : "Ganjil";
}