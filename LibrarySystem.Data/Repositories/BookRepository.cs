using LibrarySystem.Core;
using LibrarySystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Data.Repositories
{
    // Implementering av IBookRepository som hanterar CRUD-operationer för böcker i databasen
    public class BookRepository : IBookRepository
    {
        private readonly IDbContextFactory<LibraryContext> _contextFactory;

        // Konstruktor som tar emot en DbContextFactory för att skapa instanser av LibraryContext
        public BookRepository(IDbContextFactory<LibraryContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Hämta alla böcker från databasen
        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Books.ToListAsync();
        }

        // Hämta en bok baserat på dess ID
        public async Task<Book?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Books.FirstOrDefaultAsync(b => b.Id == id);
        }

        // Hämta en bok baserat på dess unika ISBN
        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        // Lägg till en ny bok i databasen
        public async Task AddAsync(Book book)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Här kan du logga felet eller hantera det på annat sätt
                throw new Exception($"Kunde inte spara boken. Detaljer: {ex.Message}");
            }
        }

        // Uppdatera en befintlig bok
        public async Task UpdateAsync(Book book)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Books.Update(book);
            await context.SaveChangesAsync();
        }

        // Ta bort en bok baserat på dess ID
        public async Task DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var book = await context.Books.FindAsync(id);
            if (book != null)
            {
                context.Books.Remove(book);
                await context.SaveChangesAsync();
            }
        }

        // Sök efter böcker baserat på titel, författare eller ISBN
        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(searchTerm)) return await GetAllAsync();
            
            var term = searchTerm.ToLower();
            return await context.Books
                .Where(b => b.Title.ToLower().Contains(term) || 
                            b.Author.ToLower().Contains(term) || 
                            b.ISBN.Contains(term))
                .ToListAsync();
        }
    }
}