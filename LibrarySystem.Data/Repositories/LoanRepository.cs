using LibrarySystem.Core;
using LibrarySystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Data.Repositories
{ // Repository-klass för att hantera databasoperationer relaterade till lån
    public class LoanRepository : ILoanRepository
    {
        private readonly IDbContextFactory<LibraryContext> _contextFactory;

        public LoanRepository(IDbContextFactory<LibraryContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Loan>> GetAllActiveLoansAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null)
                .ToListAsync();
        }

        public async Task AddLoanAsync(Loan loan)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var book = await context.Books.FindAsync(loan.BookId);
            if (book != null) book.IsAvailable = false;

            context.Loans.Add(loan);
            await context.SaveChangesAsync();
        }

        public async Task ReturnLoanAsync(int loanId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var loan = await context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == loanId);
            
            if (loan != null)
            {
                loan.ReturnDate = DateTime.Now;
                if (loan.Book != null) loan.Book.IsAvailable = true;
                await context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Loan>> GetLoanHistoryAsync()
        { // Hämtar alla lån som har returnerats, sorterade efter returdatum i fallande ordning
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate != null)
                .OrderByDescending(l => l.ReturnDate)
                .ToListAsync();
        }
    }
}