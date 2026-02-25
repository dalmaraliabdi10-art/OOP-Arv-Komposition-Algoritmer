using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Core
{
    public class Book
    {

        // Databasens ID
        public int Id { get; set; } 
        
        [Required]
        public string ISBN { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Author { get; set; } = string.Empty;
        
        public int PublishedYear { get; set; }
        
        public bool IsAvailable { get; set; } = true;

        // En bok kan ha flera lån
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}