using API.Helpers;
using Xunit;

namespace API.Tests;

public class BaseParamsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageNumberIsAtLeastOne(int pageNumber)
    {
        var parameters = new BaseParams { PageNumber = pageNumber };

        Assert.Equal(1, parameters.PageNumber);
    }

    [Theory]
    [InlineData(100, 25)]
    [InlineData(0, 12)]
    [InlineData(-1, 12)]
    public void PageSizeIsKeptWithinSupportedBounds(int pageSize, int expected)
    {
        var parameters = new BaseParams { PageSize = pageSize };

        Assert.Equal(expected, parameters.PageSize);
    }

    [Fact]
    public void TotalPagesRoundsUpForPartialPages()
    {
        var parameters = new BaseParams { PageSize = 12, TotalCount = 25 };

        Assert.Equal(3, parameters.TotalPages);
    }

    [Fact]
    public void ConstructorReadsPaginationAndSortValues()
    {
        var query = new Dictionary<string, string>
        {
            [nameof(BaseParams.PageNumber)] = "2",
            [nameof(BaseParams.PageSize)] = "10",
            [Constants.OrderBy] = "Latest"
        };

        var parameters = new BaseParams(query);

        Assert.Equal(2, parameters.PageNumber);
        Assert.Equal(10, parameters.PageSize);
        Assert.Equal("Latest", parameters.OrderBy);
    }
}
