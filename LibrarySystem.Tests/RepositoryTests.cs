using LibrarySystem.Core;
using LibrarySystem.Data;
using LibrarySystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibrarySystem.Tests
{
    // En falsk DbContextFactory för att kunna testa i minnet
    public class FakeDbContextFactory : IDbContextFactory<LibraryContext>
    {
        private readonly DbContextOptions<LibraryContext> _options;
        public FakeDbContextFactory(DbContextOptions<LibraryContext> options) => _options = options;
        public LibraryContext CreateDbContext() => new LibraryContext(_options);
    }

    public class RepositoryTests
    {
        private FakeDbContextFactory GetFactory()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ny databas för varje test
                .Options;
            return new FakeDbContextFactory(options);
        }

        [Fact]
        public async Task AddAsync_ShouldSaveBookToDatabase()
        { 
            // Förbered testdata: Skapa en ny bok och lägg till den i databasen
            var factory = GetFactory();
            var repo = new BookRepository(factory);
            await repo.AddAsync(new Book { ISBN = "123", Title = "Testbok" });
            using var context = factory.CreateDbContext();
            Assert.Equal(1, context.Books.Count());
            Assert.Equal("Testbok", context.Books.First().Title);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBooks()
        { 
            // Förbered testdata: Lägg till två böcker i databasen
            var factory = GetFactory();
            var repo = new BookRepository(factory);
            await repo.AddAsync(new Book { ISBN = "1", Title = "Bok 1" });
            await repo.AddAsync(new Book { ISBN = "2", Title = "Bok 2" });

            var books = await repo.GetAllAsync();
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingBook()
        { 
            // Förbered testdata: Lägg till en bok
            var factory = GetFactory();
            using (var context = factory.CreateDbContext())
            {
                context.Books.Add(new Book { Id = 1, ISBN = "999", Title = "Gammal Titel" });
                context.SaveChanges();
            }
            var repo = new BookRepository(factory);
            var bookToUpdate = await repo.GetByIdAsync(1);
            
            // FIX: Assert.NotNull talar om för C# att boken existerar, så slipper vi varningen!
            Assert.NotNull(bookToUpdate);
            
            bookToUpdate.Title = "Ny Titel";
            await repo.UpdateAsync(bookToUpdate);
            using var verifyContext = factory.CreateDbContext();
            Assert.Equal("Ny Titel", verifyContext.Books.First().Title);
        }

        [Fact]
        public async Task GetByISBNAsync_ShouldReturnCorrectBook()
        { 
            // Förbered testdata: Lägg till två böcker med olika ISBN
            var factory = GetFactory();
            var repo = new BookRepository(factory);
            await repo.AddAsync(new Book { ISBN = "111", Title = "Bok 1" });
            await repo.AddAsync(new Book { ISBN = "222", Title = "Bok 2" });
            var book = await repo.GetByISBNAsync("222");
            Assert.NotNull(book);
            Assert.Equal("Bok 2", book.Title);
        }

        [Fact]
        public async Task AddMemberAsync_ShouldThrowException_IfEmailExists()
        { 
            // Förbered testdata: Lägg till en medlem med en specifik e-postadress
            var factory = GetFactory();
            var repo = new MemberRepository(factory);
            await repo.AddAsync(new Member { Name = "Dalmar", Email = "test@test.com" });
            var duplicateMember = new Member { Name = "Kopia", Email = "test@test.com" };
            
            await Assert.ThrowsAsync<Exception>(() => repo.AddAsync(duplicateMember));
        }

        [Fact]
        public async Task GetAllActiveLoansAsync_ShouldNotReturnReturnedBooks()
        { 
            // Förbered testdata: Skapa en medlem och två lån, där det ena är återlämnat
            var factory = GetFactory();
            using (var context = factory.CreateDbContext())
            {
                var member = new Member { Name = "Testperson", Email = "test@test.com" };
                var book1 = new Book { ISBN = "111", Title = "Bok 1" };
                var book2 = new Book { ISBN = "222", Title = "Bok 2" };
                context.Loans.Add(new Loan { Book = book1, Member = member, ReturnDate = null });
                context.Loans.Add(new Loan { Book = book2, Member = member, ReturnDate = DateTime.Now });
                context.SaveChanges();
            }
            var repo = new LoanRepository(factory);
            var activeLoans = await repo.GetAllActiveLoansAsync();
            Assert.Single(activeLoans);
            Assert.Equal("111", activeLoans.First().Book?.ISBN);
        }

        [Fact]
        public async Task SearchAsync_ShouldFindBooksByTitle()
        { 
            // Förbered testdata: Lägg till en bok med "ringen" i titeln
            var factory = GetFactory();
            var repo = new BookRepository(factory);
            await repo.AddAsync(new Book { ISBN = "1", Title = "Sagan om ringen" });
            var results = await repo.SearchAsync("ringen");
            Assert.Single(results);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBook()
        { 
            // Förbered testdata: Lägg till en bok som sedan ska tas bort
            var factory = GetFactory();
            using (var context = factory.CreateDbContext())
            {
                context.Books.Add(new Book { Id = 1, ISBN = "123", Title = "Ta bort mig" });
                context.SaveChanges();
            }
            var repo = new BookRepository(factory);
            await repo.DeleteAsync(1);
            using var verifyContext = factory.CreateDbContext();
            Assert.Empty(verifyContext.Books);
        }

        [Fact]
        public async Task AddMemberAsync_ShouldSaveMember()
        { 
            // Förbered testdata: Skapa en ny medlem och lägg till den i databasen
            var factory = GetFactory();
            var repo = new MemberRepository(factory);
            await repo.AddAsync(new Member { Name = "Dalmar", Email = "test@test.com" });
            using var context = factory.CreateDbContext();
            Assert.Single(context.Members);
        }

        [Fact]
        public async Task AddLoanAsync_ShouldSetBookToUnavailable()
        { 
            // Förbered testdata: Lägg till en bok som är tillgänglig
            var factory = GetFactory();
            using (var context = factory.CreateDbContext())
            {
                context.Books.Add(new Book { Id = 1, ISBN = "123", Title = "Låna mig", IsAvailable = true });
                context.SaveChanges();
            }
            var repo = new LoanRepository(factory);
            await repo.AddLoanAsync(new Loan { BookId = 1, MemberId = 1 });
            using var verifyContext = factory.CreateDbContext();
            Assert.False(verifyContext.Books.First().IsAvailable); // Boken ska vara utlånad (False)
        }

        [Fact]
        public async Task ReturnLoanAsync_ShouldSetBookToAvailable()
        { 
            // Förbered testdata: Skapa ett lån där boken är utlånad
            var factory = GetFactory();
            using (var context = factory.CreateDbContext())
            {
                var book = new Book { Id = 1, ISBN = "123", Title = "Återlämna mig", IsAvailable = false };
                var loan = new Loan { Id = 1, BookId = 1, MemberId = 1, Book = book };
                context.Loans.Add(loan);
                context.SaveChanges();
            }
            var repo = new LoanRepository(factory);
            await repo.ReturnLoanAsync(1);
            using var verifyContext = factory.CreateDbContext();
            var returnedLoan = verifyContext.Loans.First();
            Assert.NotNull(returnedLoan.ReturnDate); // Har fått ett returdatum
            Assert.True(verifyContext.Books.First().IsAvailable); // Boken är tillgänglig igen
        }
    }
}