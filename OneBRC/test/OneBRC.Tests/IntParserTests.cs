using System.Text;
using OneBRC.Shared;

namespace OneBRC.Tests;

public class IntParserTests
{
    [Theory]
    [InlineData("1.000", 1000)]
    [InlineData("-273.016", -273016)]
    [InlineData("42.000", 42000)]
    [InlineData("42.023", 42023)]
    [InlineData("-42.023", -42023)]
    public void FortyTwo(string input, int expected)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        Assert.Equal(expected, IntParser.Parse(bytes));
    }
}