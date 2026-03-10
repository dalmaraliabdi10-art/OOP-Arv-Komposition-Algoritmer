using Bunit;
using LibrarySystem.Core;
using LibrarySystem.Web.Components;
using Xunit;

namespace LibrarySystem.Tests
{
    public class ComponentTests : BunitContext 
    {
        [Fact]
        public void BookCard_ShouldDisplayAvailableStatus_WhenBookIsAvailable()
        {
            var book = new Book { Title = "Testbok", Author = "Författare", IsAvailable = true };
            var cut = Render<BookCard>(parameters => parameters
                .Add(p => p.Book, book));

            Assert.Contains("🟢 Tillgänglig", cut.Markup);
            Assert.Contains("Testbok", cut.Markup);
        }

        [Fact]
        public void BookCard_ShouldDisplayBorrowedStatus_WhenBookIsUnavailable()
        {
            var book = new Book { Title = "Utlånad bok", Author = "Någon", IsAvailable = false };
            var cut = Render<BookCard>(parameters => parameters
                .Add(p => p.Book, book));

            Assert.Contains("🔴 Utlånad", cut.Markup);
            Assert.Contains("border-danger", cut.Markup);
        }
    }
}