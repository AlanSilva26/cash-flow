using CashFlow.Transaction.Domain.Enums;
using CashFlow.Transaction.Domain.Exceptions;
using FluentAssertions;
using TransactionEntity = CashFlow.Transaction.Domain.Entities.Transaction;

namespace CashFlow.Transaction.UnitTests.Domain.Entities.Transaction;

public sealed class ConstructorTests
{
    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        const decimal amount = 100.50m;
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var transaction = new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        transaction.Id.Should().Be(id);
        transaction.Amount.Should().Be(amount);
        transaction.Type.Should().Be(type);
        transaction.OccurredOn.Should().Be(occurredOn);
    }

    [Fact]
    public void Constructor_WhenIdIsEmpty_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.Empty;
        const decimal amount = 100.50m;
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        Action action = () => new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        action.Should()
              .Throw<DomainException>()
              .WithMessage("Id cannot be empty.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Constructor_WhenAmountIsNonPositive_ShouldThrowDomainException(decimal amount)
    {
        // Arrange
        var id = Guid.NewGuid();
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        Action action = () => new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        action.Should()
              .Throw<DomainException>()
              .WithMessage("Amount must be greater than zero.");
    }

    [Theory]
    [InlineData(0.001)]
    [InlineData(1.001)]
    [InlineData(10.999)]
    [InlineData(100.505)]
    public void Constructor_WhenAmountHasMoreThanTwoDecimalPlaces_ShouldThrowDomainException(decimal amount)
    {
        // Arrange
        var id = Guid.NewGuid();
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        Action action = () => new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        action.Should()
              .Throw<DomainException>()
              .WithMessage("Amount cannot have more than two decimal places.");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(1.2)]
    [InlineData(1.10)]
    [InlineData(999999999.99)]
    public void Constructor_WhenAmountIsPositiveAndHasAtMostTwoDecimalPlaces_ShouldCreateTransaction(decimal amount)
    {
        // Arrange
        var id = Guid.NewGuid();
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var transaction = new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        transaction.Amount.Should().Be(amount);
    }

    [Theory]
    [InlineData(TransactionType.Credit)]
    [InlineData(TransactionType.Debit)]
    public void Constructor_WhenTransactionTypeIsValid_ShouldCreateTransaction(TransactionType type)
    {
        // Arrange
        var id = Guid.NewGuid();
        const decimal amount = 100.50m;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var transaction = new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        transaction.Type.Should().Be(type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(999)]
    [InlineData(-1)]
    public void Constructor_WhenTransactionTypeIsInvalid_ShouldThrowDomainException(
        int invalidTypeValue)
    {
        // Arrange
        var id = Guid.NewGuid();
        const decimal amount = 100.50m;
        var type = (TransactionType)invalidTypeValue;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        Action action = () => new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        action.Should()
              .Throw<DomainException>()
              .WithMessage("Type is invalid.");
    }

    [Fact]
    public void Constructor_WhenOccurredOnIsToday_ShouldCreateTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        const decimal amount = 100.50m;
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var transaction = new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        transaction.OccurredOn.Should().Be(occurredOn);
    }

    [Fact]
    public void Constructor_WhenOccurredOnIsInThePast_ShouldCreateTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        const decimal amount = 100.50m;
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        // Act
        var transaction = new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        transaction.OccurredOn.Should().Be(occurredOn);
    }

    [Fact]
    public void Constructor_WhenOccurredOnIsInTheFuture_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        const decimal amount = 100.50m;
        const TransactionType type = TransactionType.Credit;
        var occurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        // Act
        Action action = () => new TransactionEntity(id, amount, type, occurredOn);

        // Assert
        action.Should()
              .Throw<DomainException>()
              .WithMessage("Occurred On cannot be in the future.");
    }
}