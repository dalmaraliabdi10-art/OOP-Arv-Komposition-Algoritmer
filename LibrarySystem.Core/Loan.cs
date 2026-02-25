using System;

namespace LibrarySystem.Core
{
    public class Loan
    {
        public int Id { get; set; }
        
        // För att hålla reda på vilken bok och medlem som är involverade i lånet
        public int BookId { get; set; }
        public Book? Book { get; set; } 
        
        public int MemberId { get; set; }
        public Member? Member { get; set; } 

        public DateTime LoanDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsOverdue => ReturnDate == null && DateTime.Now > DueDate;
        
        public void MarkAsReturned()
        {
            ReturnDate = DateTime.Now;
            if (Book != null)
            {
                Book.IsAvailable = true;
            }
        }
    }
}