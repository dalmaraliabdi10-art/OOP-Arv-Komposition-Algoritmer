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
