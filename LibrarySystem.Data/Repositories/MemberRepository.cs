using LibrarySystem.Core;
using LibrarySystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibrarySystem.Data.Repositories
{ // Repository-klass för att hantera databasoperationer relaterade till medlemmar
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
            context.Members.Add(member);
            await context.SaveChangesAsync();
        }
    }
}