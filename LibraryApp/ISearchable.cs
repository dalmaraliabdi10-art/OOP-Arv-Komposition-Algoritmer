namespace LibraryApp
{
    public interface ISearchable
    {
        bool Matches(string searchTerm);
    }
}