using API.Extensions;
using Xunit;

namespace API.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("10-20")]
    [InlineData("-20")]
    [InlineData("10-")]
    public void IntegerRangeAcceptsValidBounds(string value)
    {
        Assert.True(value.IsValidIntegerRange());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("one-20")]
    [InlineData("10-20-30")]
    public void IntegerRangeRejectsInvalidBounds(string value)
    {
        Assert.False(value.IsValidIntegerRange());
    }

    [Fact]
    public void TryParseLocationReturnsNameAndTypeForValidValue()
    {
        var isValid = "Seattle-City".TryParseLocation(out var name, out var type);

        Assert.True(isValid);
        Assert.Equal("Seattle", name);
        Assert.Equal("City", type);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Seattle")]
    [InlineData("Seattle-Unknown")]
    [InlineData("North-Seattle-City")]
    public void TryParseLocationRejectsInvalidValue(string value)
    {
        Assert.False(value.TryParseLocation(out _, out _));
    }
}
