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

        // Metod för att söka efter böcker baserat på en sökterm (titel, författare eller ISBN)
        public List<Book> SearchBooks(string searchTerm)
        {
            // Använder LINQ för att filtrera böcker som matchar söktermen
            return _books.Where(b => b.Matches(searchTerm)).ToList();
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
        public Member GetMostActiveMember()
        {
            if (!_loans.Any()) return null;
            // Den grupperar lånen efter medlem, sorterar grupperna i fallande ordning baserat på antal lån, och tar den medlem som har flest lån
            return _loans
                .GroupBy(l => l.Member)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        // Metod för att låna ut en bok till en medlem
        public void LoanBook(string isbn, string memberId)
        {
            var book = _books.FirstOrDefault(b => b.ISBN == isbn);
            var member = _members.FirstOrDefault(m => m.MemberId == memberId);

            if (book == null || member == null) 
            { // Om antingen boken eller medlemmen inte hittas, skriv ut ett meddelande och avsluta metoden
                Console.WriteLine("Kunde inte hitta bok eller medlem.");
                return;
            }
            
            if (!book.IsAvailable)
            { // Om boken inte är tillgänglig, skriv ut ett meddelande och avsluta metoden
                Console.WriteLine("Boken är tyvärr redan utlånad.");
                return;
            }

         
            book.IsAvailable = false;
            var loan = new Loan(book, member, DateTime.Now, DateTime.Now.AddDays(14));
            
            _loans.Add(loan);
            member.BorrowedBooks.Add(book);
            
            Console.WriteLine($"Boken '{book.Title}' lånades ut till {member.Name}.");
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