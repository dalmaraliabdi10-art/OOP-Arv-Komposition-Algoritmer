# 📚 Bibliotekssystem 2.0 (Blazor & Entity Framework Core)

Detta är Del 2 i utvecklingen av bibliotekssystemet. Applikationen har gått från att vara en konsolapplikation till en modern webbapplikation byggd med **Blazor Server** och **Entity Framework Core** (SQLite). 

Projektet är strukturerat enligt **Repository Pattern** för att separera databaslogik från gränssnitt, vilket gör systemet robust, skalbart och testbart.

---

## 🚀 Instruktioner för att köra projektet

För att köra projektet lokalt på dator behöver man ha [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (eller nyare) installerat.

**1. Klona projektet och navigera till mappen:**
```bash
git clone https://github.com/dalmaraliabdi10-art/OOP-Arv-Komposition-Algoritmer
cd OOP-Arv-Komposition-Algoritmer

2. Skapa databasen:
(Om filen library.db redan finns kan du hoppa över detta steg)
Bash
dotnet ef database update --project LibrarySystem.Data --startup-project LibrarySystem.Web

3. Starta webbapplikationen:
Bash
dotnet watch --project LibrarySystem.Web

4. Kör enhetstesterna:
Bash
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
1. Böcker:
Lagrar uppgifter om böckerna som finns i biblioteket.

Har en obligatorisk och unik indexering baserad på ISBN så att inga kopior kan skapas.

IsAvailable är en sanningsvariabel som automatiskt blir false när boken lånas.

Relation: En bok kan ha en historik av många lån

2. Medlemmar:
Medlemmar: En medlem kan göra många lån

Lagrar information om personer som använder biblioteket.

E-postadressen kontrolleras i Repository lagret för att förhindra att samma e-post används av flera medlemmar.

Relation: En medlem kan göra flera lån (En till många till Lån).

3. Lån:
Fungerar som centrum i databasen. Den kopplar ihop en Bok och en Medlem med hjälp av Foreign Keys (BookId och MemberId).

Har en valfri ReturnDate. Om detta fält är tomt betyder det att lånet är aktivt.

📸 Screenshots av Blazor-gränssnittet
Här är bilder på hur de 5 sidorna i applikationen ser ut:

1. Startsida (Dashboard)
2. Bokkatalogen
3. Medlemsregister
4. Utlåningssystem
5. Lånehistorik
 Visar en översikt över alla tidigare lån som har återlämnats till biblioteket, sorterat på datum.