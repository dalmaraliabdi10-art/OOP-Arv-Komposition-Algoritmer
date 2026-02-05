using System;

namespace LibraryApp
{
    public class Loan
    {
        public Book Book { get; set; }
        public Member Member { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } // "?" betyder att den kan vara null 

        public Loan(Book book, Member member, DateTime loanDate, DateTime dueDate)
        {
            Book = book;
            Member = member;
            LoanDate = loanDate;
            DueDate = dueDate;
        }

        // En egenskap som räknar ut svaret varje gång vi frågar efter det
        public bool IsOverdue 
        {
            get 
            { 
                // Om boken redan är återlämnad, kolla inte om den är försenad
                if (ReturnDate.HasValue) return false; 
                
                // Om boken inte är återlämnad, kolla om dagens datum är efter förfallodatumet
                return DateTime.Now > DueDate; 
            }
        }
        
        // Kollar om ReturnDate har ett värde
        public bool IsReturned => ReturnDate.HasValue; 
    }
}