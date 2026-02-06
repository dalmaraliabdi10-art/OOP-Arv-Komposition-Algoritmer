using Xunit;
using LibraryApp;

// Tester för ISearchable och Book-klassen

namespace LibraryTests
{
    public class BookTests
    {
        [Fact] //Testa att konstruktorn sätter egenskaperna korrekt
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var book = new Book("978-1", "Testbok", "Författare", 2024);
            // Assert (kolla att egenskaperna har rätt värden)
            Assert.Equal("978-1", book.ISBN);
            Assert.Equal("Testbok", book.Title);
            Assert.Equal(2024, book.PublishedYear);
        }

        [Fact] // Testa att GetInfo returnerar en sträng som innehåller titel, författare och ISBN
        public void GetInfo_ShouldReturnFormattedString()
        {
            var book = new Book("123", "Boktitel", "Förf", 2020);
            string info = book.GetInfo();
            
            // Assert - kolla att strängen innehåller rätt information
            Assert.Contains("Boktitel", info);
            Assert.Contains("Förf", info);
            Assert.Contains("123", info);
        }

        [Fact] // Testa att en ny bok är tillgänglig (IsAvailable ska vara true)
        public void IsAvailable_ShouldBeTrueForNewBook()
        {
            var book = new Book("123", "Titel", "Förf", 2020);
            Assert.True(book.IsAvailable);
        }
        // Testa Matches-metoden med olika söktermer som borde matcha titel, författare eller ISBN
        [Theory] 
        [InlineData("Harry", true)]
        [InlineData("harry", true)] // Case insensitive
        [InlineData("978", true)]   // ISBN
        public void Matches_ShouldReturnTrue_WhenTermMatches(string term, bool expected)
        {
            // Arrange
            var book = new Book("978-1", "Harry Potter", "Rowling", 1997);
            // Act
            var result = book.Matches(term);
            // Assert
            Assert.Equal(expected, result);
        }

        [Fact] // Testa Matches-metoden med en sökterm som inte borde matcha något
        public void Matches_ShouldReturnFalse_WhenNoMatch()
        {
            var book = new Book("123", "Sagan om ringen", "Tolkien", 1954); // En bok som inte har något med "Zlatan" att göra
            var result = book.Matches("Zlatan"); // Ska inte matcha
            Assert.False(result); // Förväntar oss false eftersom "Zlatan" inte finns i titel, författare eller ISBN
        }
    }
}