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
            var book = new Book("1", "T", "A", 2020);
            var member = new Member("M1", "N", "E");
            // Skulle lämnats tillbaka om 14 dagar
            var loan = new Loan(book, member, DateTime.Now, DateTime.Now.AddDays(14));
            // Act & Assert (kolla att det inte är försenat)
            Assert.False(loan.IsOverdue);
        }

        [Fact]
        public void IsOverdue_ShouldReturnTrue_WhenDueDateHasPassed()
        {
            // Arrange
            var book = new Book("1", "T", "A", 2020);
            var member = new Member("M1", "N", "E");
            // Skulle lämnats tillbaka för 1 dag sedan
            var loan = new Loan(book, member, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-1));
            // Act & Assert
            Assert.True(loan.IsOverdue);
        }

        [Fact]
        public void IsReturned_ShouldReturnTrue_WhenReturnDateIsSet()
        {
            // Arrange
            var book = new Book("1", "T", "A", 2020);
            var member = new Member("M1", "N", "E");
            // Har returnDate som betyder att boken är återlämnad
            var loan = new Loan(book, member, DateTime.Now, DateTime.Now.AddDays(14));
            loan.ReturnDate = DateTime.Now; // Sätter ReturnDate för att markera att boken är återlämnad
            // Act & Assert
            Assert.True(loan.IsReturned);
        }
    }
}