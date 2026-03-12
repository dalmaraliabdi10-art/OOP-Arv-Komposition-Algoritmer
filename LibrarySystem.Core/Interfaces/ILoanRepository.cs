using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibrarySystem.Core.Interfaces
{
    public interface ILoanRepository
    {
        Task<IEnumerable<Loan>> GetAllActiveLoansAsync();
        Task AddLoanAsync(Loan loan);
        Task ReturnLoanAsync(int loanId);
        Task<IEnumerable<Loan>> GetLoanHistoryAsync();
    }
}