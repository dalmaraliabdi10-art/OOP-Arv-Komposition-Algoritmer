using System;
using System.Collections.Generic; // För List<T>

namespace LibraryApp
{
    public class Member
    {
        public string MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime MemberSince { get; set; }
        
        // Lista för att hålla reda på vilka böcker medlemmen har
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();

        public Member(string id, string name, string email)
        {
            MemberId = id;
            Name = name;
            Email = email;
            MemberSince = DateTime.Now;
        }

        public void ShowInfo() // Metod för att visa medlemsinformation
        {
            Console.WriteLine($"Medlem: {Name} ({Email}) - ID: {MemberId}");
            Console.WriteLine($"Antal lånade böcker: {BorrowedBooks.Count}");
        }
    }
}