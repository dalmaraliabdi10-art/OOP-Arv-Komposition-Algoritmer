using Xunit;
using LibraryApp; 

namespace LibraryTests
{ // Tests for the Book class
    public class BookTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            string isbn = "978-1-23";
            string title = "Testboken";
            string author = "Test Testsson";
            int year = 2024;

            // Act
            var book = new Book(isbn, title, author, year);

            // Assert
            Assert.Equal(isbn, book.ISBN);
            Assert.Equal(title, book.Title);
            Assert.Equal(author, book.Author);
            Assert.Equal(year, book.PublishedYear);
            Assert.True(book.IsAvailable);
        }
    }
} 