using System;
using System.Collections.Generic;

namespace LibraryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            InitializeData(library);

            bool running = true;

            while (running)
            {
                // Visa huvudmenyn
                Console.Clear();
                Console.WriteLine("=== Bibliotekssystem ===");
                Console.WriteLine("1. Visa alla böcker");
                Console.WriteLine("2. Sök bok");
                Console.WriteLine("3. Låna bok");
                Console.WriteLine("4. Returnera bok");
                Console.WriteLine("5. Visa medlemmar");
                Console.WriteLine("6. Statistik");
                Console.WriteLine("0. Avsluta");
                Console.WriteLine("------------------------"); // Linje för att separera menyn från input
                Console.Write("Välj: ");

                string choice = Console.ReadLine() ?? string.Empty;
                Console.WriteLine(); // Extra rad för att ge lite luft efter användarens val

                try
                {
                    switch (choice)
                    {
                        case "1":
                            var allBooks = library.SearchBooks("");
                            Console.WriteLine("--- Alla böcker ---");
                            foreach (var book in allBooks)
                            {
                                string status = book.IsAvailable ? "Tillgänglig" : "Utlånad";
                                Console.WriteLine($"{book.GetInfo()} - [{status}]");
                            }
                            WaitForKey();
                            break;

                        case "2": // Sök bok
                            Console.Write("Ange sökterm (Titel/Författare/ISBN): ");
                            string term = Console.ReadLine() ?? string.Empty;
                            var results = library.SearchBooks(term);
                            
                            Console.WriteLine($"\nHittade {results.Count} böcker:");
                            foreach (var book in results)
                            {
                                Console.WriteLine(book.GetInfo());
                            }
                            WaitForKey();
                            break;

                        case "3": // Låna bok
                            Console.Write("Ange ISBN på boken: ");
                            string isbnToLoan = Console.ReadLine() ?? string.Empty;
                            Console.Write("Ange Medlems-ID: ");
                            string memberId = Console.ReadLine() ?? string.Empty;

                            // Försök att låna boken, om det inte går så kommer ett undantag att kastas som fångas i catch-blocket
                            library.LoanBook(isbnToLoan, memberId);
                            
                            // Om man kommer hit så har lånet lyckats, visa en bekräftelse
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Lånet registrerat!");
                            Console.ResetColor();
                            WaitForKey();
                            break;

                        case "4": // Returnera bok
                            Console.Write("Ange ISBN på boken som ska återlämnas: ");
                            string isbnToReturn = Console.ReadLine() ?? string.Empty;

                            library.ReturnBook(isbnToReturn);
                            WaitForKey();
                            break;

                        case "5": // Visa medlemmar
                            var members = library.GetMembers();
                            Console.WriteLine("--- Medlemsregister ---");
                            foreach (var m in members)
                            {
                                m.ShowInfo(); // Anropar ShowInfo-metoden på varje medlem för att visa deras information
                            }
                            WaitForKey();
                            break;

                        case "6": // Statistik
                            Console.WriteLine("--- Biblioteksstatistik ---");
                            Console.WriteLine($"Totalt antal böcker: {library.GetTotalBooksCount()}");
                            Console.WriteLine($"Antal utlånade böcker: {library.GetBorrowedBooksCount()}");
                            // Hitta och visa den mest aktiva medlemmen
                            var topMember = library.GetMostActiveMember();
                            if (topMember != null)
                            {
                                Console.WriteLine($"Mest aktiva låntagare: {topMember.Name}");
                            }
                            else
                            {
                                Console.WriteLine("Mest aktiva låntagare: Ingen data än");
                            }
                            WaitForKey();
                            break;

                        case "0":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Ogiltigt val, försök igen.");
                            WaitForKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Om något går fel (t.ex. bok inte hittad, bok redan utlånad, medlem inte hittad), fånga undantaget och visa ett felmeddelande i rött
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FEL: {ex.Message}");
                    Console.ResetColor();
                    WaitForKey();
                }
            }
        }

        // En metod för att pausa programmet tills användaren trycker på en tangent
        static void WaitForKey()
        {
            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        // En metod för att lägga till några böcker och medlemmar i biblioteket så att vi har data att arbeta med när programmet startar
        static void InitializeData(Library lib)
        {
            // Lägg till böcker
            lib.AddBook(new Book("978-1", "Sagan om Ringen", "J.R.R. Tolkien", 1954));
            lib.AddBook(new Book("978-2", "Hobbiten", "J.R.R. Tolkien", 1937));
            lib.AddBook(new Book("978-3", "Harry Potter", "J.K. Rowling", 1997));
            lib.AddBook(new Book("978-4", "C# i Nötskal", "Albahari", 2022));
            lib.AddBook(new Book("978-5", "Clean Code", "Robert C. Martin", 2008));

            // Lägg till medlemmar
            lib.AddMember(new Member("M001", "Anna Andersson", "anna@example.com"));
            lib.AddMember(new Member("M002", "Bengt Berg", "bengt@example.com"));
            lib.AddMember(new Member("M003", "Cecilia Ceder", "cecilia@example.com"));
        }
    }
}