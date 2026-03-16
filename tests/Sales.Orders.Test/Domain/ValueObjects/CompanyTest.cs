using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Test.Domain.ValueObjects;

public sealed class CompanyTest
{
    [Fact(DisplayName = "Should create a valid company")]
    public void T1()
    {
        var company = Company.Create(id: Guid.NewGuid(), name: "Company teste");
        Assert.NotNull(company.Value);
        Assert.Empty(company.Errors);
    }

    [Fact(DisplayName = "Should fail for an empty GUID.")]
    public void T2()
    {
        var company = Company.Create(id: Guid.Empty, name: "Company teste");
        Assert.Null(company.Value);
        Assert.Equal("Company Id cannot be empty.", company.Errors.First().Message);
    }

    [Theory(DisplayName = "Should fail for an empty name.")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void T3(string value)
    {
        var company = Company.Create(id: Guid.NewGuid(), name: value);
        Assert.Null(company.Value);
        Assert.Equal("Company name is required.", company.Errors.First().Message);
    }
}