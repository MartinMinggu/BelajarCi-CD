namespace MathService.Tests;

public class MathTest
{
    [Fact]
    public void Add_TwoNumber_ReturnsCorrectSum()
    {
        var result = MathLogic.Add(1, 2);
        Assert.Equal(3, result);
    }
    [Fact]
    public void Multiply_TwoNumber_ReturnsCorrectSum()
    {
        var result = MathLogic.Multiply(2, 2);
        Assert.Equal(4, result);
    }

    // kalo salah error
    // [Fact]
    // public void Add_NegativeNumbers_Works()
    // {
    //     var result = MathLogic.Add(-3, -7);
    //     Assert.Equal(-1, result);
    // }
    [Fact]
    public void Add_NegativeNumbers_Works()
    {
        var result = MathLogic.Add(-3, -7);
        Assert.Equal(-10, result);
    }
}