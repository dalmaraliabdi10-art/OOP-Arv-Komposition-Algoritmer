namespace LibraryApp
{ // Ett enkelt interface som kan implementeras av klasser som vill vara sökbara
    public interface ISearchable
    {
        bool Matches(string searchTerm);
    }
}