using FluentAssertions;
using Moq;
using System;
using TradeReportETL.TransactionCost.Services.TransactionCost;
using Xunit;

namespace TradeReportETL.TransactionCost.UnitTests;

public class TransactionCostCalculatorServiceTests
{

    [Theory]
    [InlineData("BE", 1, 1)]
    [InlineData("US", 1, 1)]
    public void CalculateTransactionCost_WithUnsupportedCountry_Throws(string country, decimal rate, decimal notional)
    {
        var _sut = new TransactionCostCalculatorService();

        var act = () => _sut.CalculateTransactionCost(country, rate, notional);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("NL", 1, 1000, 0)]
    [InlineData("GB", 1, 1000, 0)]
    [InlineData("NL", 0.5, 1000, 500)]
    [InlineData("GB", 0.5, 1000, -500)]
    public void CalculateTransactionCost_WithValidValues_ShouldReturnCorrectCosts(string country, decimal rate, decimal notional, decimal expectedCosts)
    {

        var _sut = new TransactionCostCalculatorService();

        var costs = _sut.CalculateTransactionCost(country, rate, notional);

        costs.Should().BeApproximately(expectedCosts, precision: 0.000001m);
    }

}