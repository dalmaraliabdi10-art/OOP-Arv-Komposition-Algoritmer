namespace LibraryApp
{
    public class Book : ISearchable
    {
        // Properties
        public string ISBN { get; private set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishedYear { get; set; }
        public bool IsAvailable { get; set; }

        // Konstruktor
        public Book(string isbn, string title, string author, int year)
        {
            ISBN = isbn;
            Title = title;
            Author = author;
            PublishedYear = year;
            IsAvailable = true; // Standardvärde
        }

        // Metod för att få bokinformation
        public string GetInfo()
        {
            return $"{Title} av {Author} ({PublishedYear}) - ISBN: {ISBN}";
        }

        // Implementering av ISearchable
        public bool Matches(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return false;

            string term = searchTerm.ToLower();
            
            // Sök titel, författare eller ISBN
            return Title.ToLower().Contains(term) || 
                   Author.ToLower().Contains(term) || 
                   ISBN.Contains(term);
        }
    }
}