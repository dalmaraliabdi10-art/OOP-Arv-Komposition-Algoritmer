using System;
using System.Collections.Generic;
using System.Linq; // För List<T> och LINQ-metoder som Where, FirstOrDefault, GroupBy, etc.

namespace LibraryApp
{
    public class Library
    {
        // Listor för att hålla reda på böcker, medlemmar och lån
        private List<Book> _books = new List<Book>();
        private List<Member> _members = new List<Member>();
        private List<Loan> _loans = new List<Loan>();

        // Metoder för att lägga till böcker och medlemmar i biblioteket
        public void AddBook(Book book) => _books.Add(book);
        public void AddMember(Member member) => _members.Add(member);

        public List<Member> GetMembers() => _members; // Metod för att hämta alla medlemmar (för att visa i programmet)

        // Metod för att söka efter böcker baserat på en sökterm (titel, författare eller ISBN)
        public List<Book> SearchBooks(string searchTerm)
        {
            // Om searchTerm är null, använd en tom sträng för att undvika null-referensfel
            string term = searchTerm ?? string.Empty;
            return _books.Where(b => b.Matches(term)).ToList();
        }

        // Metod för att sortera böcker efter publiceringsår
        public List<Book> GetBooksSortedByYear()
        {
            // OrderBy sorterar listan
            return _books.OrderBy(b => b.PublishedYear).ToList();
        }

        // Metod för att räkna det totala antalet böcker i biblioteket
        public int GetTotalBooksCount() => _books.Count;
        
        public int GetBorrowedBooksCount() => _books.Count(b => !b.IsAvailable);

        // Metod för att hitta den mest aktiva medlemmen (den som har lånat flest böcker)
        public Member? GetMostActiveMember()
        {
            // Om det inte finns några lån, returnera null
            if (!_loans.Any()) return null;
            return _loans
                .GroupBy(l => l.Member)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        // Metod för att låna ut en bok till en medlem
        public void LoanBook(string isbn, string memberId)
        {
            if (string.IsNullOrEmpty(isbn)) throw new ArgumentException("ISBN kan inte vara tomt.");
            if (string.IsNullOrEmpty(memberId)) throw new ArgumentException("Medlems-ID kan inte vara tomt.");

            var book = _books.FirstOrDefault(b => b.ISBN == isbn);
            var member = _members.FirstOrDefault(m => m.MemberId == memberId);
            
            // Kolla att både boken och medlemmen finns, och att boken är tillgänglig
            if (book == null) throw new ArgumentException($"Boken med ISBN {isbn} hittades inte.");
            if (member == null) throw new ArgumentException($"Medlem med ID {memberId} hittades inte.");
            if (!book.IsAvailable) throw new InvalidOperationException("Boken är redan utlånad.");

         
            book.IsAvailable = false;
            var loan = new Loan(book, member, DateTime.Now, DateTime.Now.AddDays(14));
            
            _loans.Add(loan);
            member.BorrowedBooks.Add(book);

        }

        // Metod för att återlämna en bok
        public void ReturnBook(string isbn)
        {

            // Hitta det aktiva lånet för den här boken (om det finns)
            var loan = _loans.FirstOrDefault(l => l.Book.ISBN == isbn && l.ReturnDate == null);

            if (loan != null)
            { // Markera lånet som återlämnat genom att sätta ReturnDate, göra boken tillgänglig igen och ta bort den från medlemmens lista över lånade böcker
                loan.ReturnDate = DateTime.Now;
                loan.Book.IsAvailable = true;
                loan.Member.BorrowedBooks.Remove(loan.Book);

                Console.WriteLine($"Boken '{loan.Book.Title}' har återlämnats.");
            }
            else
            { // Om det inte finns något aktivt lån för den här boken, skriv ut ett meddelande
                Console.WriteLine("Hittade inget aktivt lån för denna bok.");
            }
        }
    }
}