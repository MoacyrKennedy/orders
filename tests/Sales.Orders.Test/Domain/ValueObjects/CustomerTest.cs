using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Test.Domain.ValueObjects;

public sealed class CustomerTest
{
    [Fact(DisplayName = "Should create a valid customer")]
    public void T1()
    {
        var company = Customer.Create(id: Guid.NewGuid(), name: "customer teste");
        Assert.NotNull(company.Value);
        Assert.Empty(company.Errors);
    }

    [Fact(DisplayName = "Should fail for an empty GUID.")]
    public void T2()
    {
        var company = Customer.Create(id: Guid.Empty, name: "customer teste");
        Assert.Null(company.Value);
        Assert.Equal("Customer Id cannot be empty.", company.Errors.First().Message);
    }

    [Theory(DisplayName = "Should fail for an empty name.")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void T3(string value)
    {
        var company = Customer.Create(id: Guid.NewGuid(), name: value);
        Assert.Null(company.Value);
        Assert.Equal("Customer name is required.", company.Errors.First().Message);
    }
}