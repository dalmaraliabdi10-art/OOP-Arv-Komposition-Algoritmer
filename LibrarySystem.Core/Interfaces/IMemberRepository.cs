using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibrarySystem.Core.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllAsync();
        Task AddAsync(Member member);
        Task DeleteAsync(int id);
    }
}