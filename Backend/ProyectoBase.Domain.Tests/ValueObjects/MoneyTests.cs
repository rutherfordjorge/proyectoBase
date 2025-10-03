using FluentAssertions;
using ProyectoBase.Api.Domain.Exceptions;
using ProyectoBase.Api.Domain.ValueObjects;
using Xunit;

namespace ProyectoBase.Api.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void From_ShouldRoundToTwoDecimals()
    {
        var money = Money.From(10.129m);

        money.Amount.Should().Be(10.13m);
    }

    [Fact]
    public void From_ShouldThrowWhenAmountIsNegative()
    {
        var action = () => Money.From(-0.01m);

        action.Should().Throw<ValidationException>()
            .WithMessage("El precio del producto no puede ser negativo.");
    }
}
