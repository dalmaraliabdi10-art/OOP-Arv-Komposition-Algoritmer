using Xunit;
using LibraryApp;
using System;

namespace LibraryTests
{

public class LoanTests
{
    [Fact]
    public void IsOverdue_ShouldReturnFalse_WhenDueDateIsInFuture()
    {
        // Arrange
        var book = new Book("123", "Test", "Author", 2024);
        var member = new Member("M001", "Test Person", "test@test.com");
        var loan = new Loan(book, member, DateTime.Now, DateTime.Now.AddDays(14));

        // Act & Assert
        Assert.False(loan.IsOverdue);
    }

    [Fact]
    public void IsOverdue_ShouldReturnTrue_WhenDueDateHasPassed()
    {
        // Testa med ett förfallet lån
    }

    [Fact]
    public void IsReturned_ShouldReturnTrue_WhenReturnDateIsSet()
    {
        // Testa att IsReturned fungerar korrekt
    }
}

} 