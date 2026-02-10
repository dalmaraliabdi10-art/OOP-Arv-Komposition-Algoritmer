using System;
using System.Collections.Generic; // För att kunna använda List<T>
using System.Linq; // För att kunna använda LINQ-metoder som FirstOrDefault, GroupBy, OrderByDescending, etc.
using System.IO; // För att kunna använda File-klassen för att läsa/skriva filer
using System.Text.Json; // För att kunna använda JsonSerializer

namespace LibraryApp
{
    public class Library
    {
        // Interna listor för att hålla reda på böcker, medlemmar och lån
        private List<Book> _books = new List<Book>();
        private List<Member> _members = new List<Member>();
        private List<Loan> _loans = new List<Loan>();
        
        // Filnamn för att spara/ladda data
        private string fileName = "library_data.json";

        public void AddBook(Book book) => _books.Add(book);
        public void AddMember(Member member) => _members.Add(member);
        public List<Member> GetMembers() => _members;

        // En wrapper-klass som hjälper med att organisera data när vi sparar/laddar
        private class LibraryDataWrapper
        {
            public List<Book> Books { get; set; } = new List<Book>();
            public List<Member> Members { get; set; } = new List<Member>();
            public List<Loan> Loans { get; set; } = new List<Loan>();
        }

        // SPARA DATA
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

        // LÄSA DATA
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

                    // Eftersom vi sparar referenser i lån, behöver vi "koppla ihop" dem igen efter att ha laddat data. Vi letar upp de riktiga objekten i våra listor och uppdaterar referenserna i lånen.
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

        // SÖK & SORTERA
        public List<Book> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return _books.ToList();
            return _books.Where(b => b.Matches(searchTerm)).ToList();
        }

        // Sorterar böckerna i biblioteket efter publiceringsår
        public List<Book> GetBooksSortedByYear() => _books.OrderBy(b => b.PublishedYear).ToList();
        public int GetTotalBooksCount() => _books.Count;
        public int GetBorrowedBooksCount() => _books.Count(b => !b.IsAvailable);

        public Member? GetMostActiveMember()
        {
            if (!_loans.Any()) return null;
            return _loans.GroupBy(l => l.Member)
                         .OrderByDescending(g => g.Count())
                         .Select(g => g.Key)
                         .FirstOrDefault();
        }

        public Loan? GetActiveLoan(string isbn)
        {
            return _loans.FirstOrDefault(l => l.Book.ISBN == isbn && l.ReturnDate == null);
        }

        // UTLÅN & ÅTERLÄMNING
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

            // Skapa ett lån och uppdatera status
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