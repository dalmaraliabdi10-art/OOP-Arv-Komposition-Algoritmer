using System;
using System.Collections.Generic; // För List<T>
using System.Linq; // För LINQ-metoder
using System.IO; // För filhantering
using System.Text.Json; // För JSON-serialisering

namespace LibraryApp
{
    public class Library
    {
        // Listor för att hålla reda på böcker, medlemmar och lån
        private List<Book> _books = new List<Book>();
        private List<Member> _members = new List<Member>();
        private List<Loan> _loans = new List<Loan>();
        
        // Filen hamnar oftast i bin/Debug/net10.0/ när du kör via Visual Studio
        private string fileName = "library_data.json";

        // Metoder för att lägga till böcker och medlemmar
        public void AddBook(Book book) => _books.Add(book);
        public void AddMember(Member member) => _members.Add(member);
        public List<Member> GetMembers() => _members;

        // En privat klass för att hjälpa till med JSON-serialisering
        private class LibraryDataWrapper
        {
            public List<Book> Books { get; set; } = new List<Book>();
            public List<Member> Members { get; set; } = new List<Member>();
            public List<Loan> Loans { get; set; } = new List<Loan>();
        }
        
        // Metod för att spara data till fil
        public void SaveData()
        {
            var dataToSave = new
            {
                Books = _books,
                Members = _members,
                Loans = _loans
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(dataToSave, options);
            File.WriteAllText(fileName, jsonString);
            Console.WriteLine("Data sparad till fil!");
        }

        // Metod för att ladda data från fil
        public void LoadData()
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Ingen sparad fil hittades. Startar med tom/test-data.");
                return;
            }

            try 
            {
                string jsonString = File.ReadAllText(fileName);
                var data = JsonSerializer.Deserialize<LibraryDataWrapper>(jsonString);
                
                if (data != null)
                {
                    _books = data.Books ?? new List<Book>();
                    _members = data.Members ?? new List<Member>();
                    _loans = data.Loans ?? new List<Loan>();
                    // Återställ referenser mellan lån, böcker och medlemmar
                    foreach (var loan in _loans)
                    {
                        var realBook = _books.FirstOrDefault(b => b.ISBN == loan.Book.ISBN);
                        if (realBook != null) loan.Book = realBook;

                        var realMember = _members.FirstOrDefault(m => m.MemberId == loan.Member.MemberId);
                        if (realMember != null) loan.Member = realMember;
                    }

                    Console.WriteLine("Data laddad från fil!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kunde inte ladda filen: {ex.Message}");
            }
        }

        // Metod för att söka böcker baserat på ett sökord
        public List<Book> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return _books.ToList();
            return _books.Where(b => b.Matches(searchTerm)).ToList();
        }

        // Metod för att få en lista med böcker sorterade efter publiceringsår
        public List<Book> GetBooksSortedByYear() => _books.OrderBy(b => b.PublishedYear).ToList();
        public int GetTotalBooksCount() => _books.Count;
        public int GetBorrowedBooksCount() => _books.Count(b => !b.IsAvailable);

        // Hitta den mest aktiva medlemmen baserat på antal lån
        public Member? GetMostActiveMember()
        {
            if (!_loans.Any()) return null;
            return _loans.GroupBy(l => l.Member)
                         .OrderByDescending(g => g.Count())
                         .Select(g => g.Key)
                         .FirstOrDefault();
        }

        // Hitta ett aktivt lån baserat på ISBN
        public Loan? GetActiveLoan(string isbn)
        {
            return _loans.FirstOrDefault(l => l.Book.ISBN == isbn && l.ReturnDate == null);
        }

        // Metod för att låna en bok
        public void LoanBook(string isbn, string memberIdOrName)
        {
            if (string.IsNullOrEmpty(isbn)) throw new ArgumentException("ISBN får inte vara tomt.");
            if (string.IsNullOrEmpty(memberIdOrName)) throw new ArgumentException("Medlems-info får inte vara tomt.");

            var book = _books.FirstOrDefault(b => b.ISBN == isbn);
            var member = _members.FirstOrDefault(m => 
                m.MemberId == memberIdOrName || 
                m.Name.ToLower().Contains(memberIdOrName.ToLower()));
            
            if (book == null) throw new ArgumentException($"Boken med ISBN {isbn} hittades inte.");
            if (member == null) throw new ArgumentException($"Ingen medlem hittades: {memberIdOrName}");
            if (!book.IsAvailable) throw new InvalidOperationException("Boken är redan utlånad.");

            book.IsAvailable = false;
            var loan = new Loan(book, member, DateTime.Now, DateTime.Now.AddDays(14));
            
            _loans.Add(loan);
            member.BorrowedBooks.Add(book);
            
            Console.WriteLine($"Lån registrerat för {member.Name}!");
            
            SaveData();
        }

        // Metod för att returnera en bok
        public void ReturnBook(string isbn)
        {
            var loan = _loans.FirstOrDefault(l => l.Book.ISBN == isbn && l.ReturnDate == null);

            if (loan != null)
            {
                loan.ReturnDate = DateTime.Now;
                loan.Book.IsAvailable = true;
                loan.Member.BorrowedBooks.Remove(loan.Book);

                Console.WriteLine($"Boken '{loan.Book.Title}' har återlämnats.");
                
                SaveData();
            }
            else
            {
                Console.WriteLine("Hittade inget aktivt lån för denna bok.");
            }
        }
    }
}