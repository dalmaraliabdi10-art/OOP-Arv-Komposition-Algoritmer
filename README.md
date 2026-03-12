Bibliotekssystem
Ett konsolbaserat program skrivet i C# för att hantera utlåning av böcker. Projektet visar objektorientering, algoritmer och datalagring.

Funktioner
Låna & Returnera: Hanterar utlåning med förfallodatum och status.

Sökfunktion: Sök på titel, författare eller ISBN.

Datalagring: Allt sparas automatiskt i library_data.json så inget försvinner. (Detta funkar inte efter man har stängt programmet)

Användarvänligt: Färgkodad text och inmatning

Så här kör du programmet
Öppna terminalen i projektmappen.

Gå in i app-mappen:
cd LibraryApp

Starta:
dotnet run

Projektet innehåller xUnit-tester för att säkra logiken.
dotnet test



Databasdiagram

erDiagram
    BOOK ||--o{ LOAN : "lånas ut i"
    MEMBER ||--o{ LOAN : "gör"

    BOOK {
        int Id PK
        string ISBN "Unique"
        string Title
        string Author
        int PublishedYear
        bool IsAvailable
    }

    MEMBER {
        int Id PK
        string Name
        string Email "Unique"
        DateTime MemberSince
    }

    LOAN {
        int Id PK
        int BookId FK
        int MemberId FK
        DateTime LoanDate
        DateTime DueDate
        DateTime ReturnDate "Nullable"
    }

Beskrivning av tabellstrukturen
Böcker:
Lagrar uppgifter om böckerna som finns i biblioteket.

Har en obligatorisk och unik indexering baserad på ISBN så att inga kopior kan skapas.

IsAvailable är en sanningsvariabel som automatiskt blir false när boken lånas.

Relation: En bok kan ha en historik av många lån

Medlemmar: En medlem kan göra många lån

Lagrar information om personer som använder biblioteket.

E-postadressen kontrolleras i Repository lagret för att förhindra att samma e-post används av flera medlemmar.

Relation: En medlem kan göra flera lån (En till många till Lån).

Lån:
Fungerar som centrum i databasen. Den kopplar ihop en Bok och en Medlem med hjälp av Foreign Keys (BookId och MemberId).

Har en valfri ReturnDate. Om detta fält är tomt betyder det att lånet är aktivt.