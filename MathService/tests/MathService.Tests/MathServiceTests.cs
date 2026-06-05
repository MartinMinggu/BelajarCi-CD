using MathService.Services;

namespace MathService.Tests;

public class MathServiceTests
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

    [Fact]
    public void Divide_ValidNumbers_ReturnsCorrectResult()
        => Assert.Equal(2.5, MathLogic.Divide(5, 2));

    [Fact]
    public void Divide_ByZero_ThrowsException()
        => Assert.Throws<DivideByZeroException>(() => MathLogic.Divide(10, 0));

    [Theory]
    [InlineData(2, true)]
    [InlineData(7, true)]
    [InlineData(13, true)]
    [InlineData(4, false)]
    [InlineData(1, false)]
    public void IsPrime_ReturnsCorrectResult(int n, bool expected)
        => Assert.Equal(expected, MathLogic.IsPrime(n));
}