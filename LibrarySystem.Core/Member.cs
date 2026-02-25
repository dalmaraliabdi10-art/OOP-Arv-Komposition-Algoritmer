using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Core
{
    public class Member
    {

        // Databasens ID
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public DateTime MemberSince { get; set; } = DateTime.Now;

        // En medlem kan ha flera lån
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}