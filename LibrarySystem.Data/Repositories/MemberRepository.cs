using LibrarySystem.Core;
using LibrarySystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibrarySystem.Data.Repositories
{ 
    // Repository-klass för att hantera databasoperationer relaterade till medlemmar
    public class MemberRepository : IMemberRepository
    {
        private readonly IDbContextFactory<LibraryContext> _contextFactory;

        public MemberRepository(IDbContextFactory<LibraryContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Member>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Members.Include(m => m.Loans).ToListAsync();
        }

        public async Task AddAsync(Member member)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var emailExists = await context.Members.AnyAsync(m => m.Email.ToLower() == member.Email.ToLower());
            if (emailExists)
            {
                throw new Exception("En medlem med denna e-postadress finns redan registrerad.");
            }
            context.Members.Add(member);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        { 
            // För att ta bort en medlem måste vi först kontrollera att de inte har några aktiva lån
            using var context = await _contextFactory.CreateDbContextAsync();
            var member = await context.Members.Include(m => m.Loans).FirstOrDefaultAsync(m => m.Id == id);
            
            if (member != null)
            {
                if (member.Loans.Any(l => l.ReturnDate == null))
                {
                    throw new Exception("Kan inte ta bort en medlem som har aktiva lån. Återlämna böckerna först!");
                }
                context.Members.Remove(member);
                await context.SaveChangesAsync();
            }
        }
    }
}