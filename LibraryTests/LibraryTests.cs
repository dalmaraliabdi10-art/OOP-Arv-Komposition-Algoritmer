using Xunit;
using LibraryApp;
using System.Linq;

namespace LibraryTests
{
    public class LibraryTests
    {
        [Fact]
        public void GetTotalBooks_ShouldReturnCorrectCount()
        {
            // Arrange (förbered testdata)
            var lib = new Library();
            // Den lägger till 2 böcker i biblioteket
            lib.AddBook(new Book("1", "B1", "A", 2020));
            // Den lägger till en till bok för att testa att det räknas korrekt
            lib.AddBook(new Book("2", "B2", "A", 2020));
            // Act & Assert (kolla att det är 2 böcker i biblioteket)
            Assert.Equal(2, lib.GetTotalBooksCount());
        }

        [Fact]
        public void GetMostActiveMember_ShouldReturnWinner()
        {
            // Arrange 
            var lib = new Library();
            // Den lägger till två medlemmar och två böcker, där Anna lånar båda böckerna och Bertil lånar ingen. Anna borde vinna "mest aktiva medlem" eftersom hon har lånat flest böcker.
            var m1 = new Member("M1", "Anna", "a@a.com");
            var m2 = new Member("M2", "Bertil", "b@b.com");
            // Lägger till medlemmarna i biblioteket
            lib.AddMember(m1);
            lib.AddMember(m2);
            // Lägger till två böcker i biblioteket
            lib.AddBook(new Book("1", "Bok1", "A", 2000));
            lib.AddBook(new Book("2", "Bok2", "A", 2000));

            // Anna lånar båda böckerna
            lib.LoanBook("1", "M1");
            lib.LoanBook("2", "M1");

            var winner = lib.GetMostActiveMember();
            Assert.Equal("Anna", winner.Name);
        }

        [Fact]
        public void SortBooksByYear_ShouldReturnOrderedList()
        {
            var lib = new Library();
            // Lägger till tre böcker med olika årtal
            lib.AddBook(new Book("1", "Ny", "A", 2024));
            lib.AddBook(new Book("2", "Gammal", "B", 1990));
            lib.AddBook(new Book("3", "Mellan", "C", 2010));

            var sorted = lib.GetBooksSortedByYear();

            // Första boken ska vara den äldsta (1990)
            Assert.Equal(1990, sorted[0].PublishedYear);
            // Sista boken ska vara den nyaste (2024)
            Assert.Equal(2024, sorted[2].PublishedYear);
        }
    }
}