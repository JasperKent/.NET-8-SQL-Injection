using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SqlInjection.DataAccess;
using System.Data.SqlClient;
namespace SqlInjection.Endpoints;

public static class BookEndpoints
{
    private readonly static Book[] _books =
    {
            new Book { Title = "Dr No", AuthorFirstName = "Ian", AuthorLastName = "Fleming"},
            new Book { Title = "Goldfinger", AuthorFirstName = "Ian", AuthorLastName = "Fleming"},
            new Book { Title = "Twelve", AuthorFirstName = "Jasper", AuthorLastName = "Kent"},
            new Book { Title = "Late Whitsun", AuthorFirstName = "Jasper", AuthorLastName = "Kent"},
            new Book { Title = "Emma", AuthorFirstName = "Jane", AuthorLastName = "Austen"},
            new Book { Title = "Persuasion", AuthorFirstName = "Jane", AuthorLastName = "Austen"},
            new Book { Title = "Master and Commander", AuthorFirstName = "Patrick", AuthorLastName = "O'Brian"},
            new Book { Title = "The Far Side of the World", AuthorFirstName = "Patrick", AuthorLastName = "O'Brian" }
    };

    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book").WithTags(nameof(Book));

        group.MapGet("/", async (BookContext context) => await context.Books.ToArrayAsync());

        group.MapPost("/Generate", async (BookContext context) =>
        {
            await context.Database.EnsureCreatedAsync();
            await context.Books.AddRangeAsync(_books);
            await context.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapGet("/ByAuthorLastNameConcatenation/{name}", (BookContext context, string name) =>
        {
            int x = 100;
            string s = "Hello";

            FormattableString interpolated = $"x is {x}. s is {s}.";

            return context.Database.SqlQueryRaw<Book>("SELECT * FROM Books WHERE AuthorLastName = '" + name + "'");
        });

        group.MapGet("/ByAuthorLastNameParameters/{name}", (BookContext context, string name) =>
        {
            return context.Database.SqlQueryRaw<Book>("SELECT * FROM Books WHERE AuthorLastName = @name", new SqliteParameter("name", name));
        });

        group.MapGet("/ByAuthorLastNameLinq/{name}", (BookContext context, string name) =>
        {
            return from b in context.Books where b.AuthorLastName == name select b;
        });

        group.MapGet("/ByAuthorLastNameInterpolation/{name}", (BookContext context, string name) =>
        {
            string query = $"SELECT * FROM Books WHERE AuthorLastName = '{name}'";

            return context.Database.SqlQueryRaw<Book>(query);
        });

        group.MapGet("/ByAuthorLastNameFormattable/{name}", (BookContext context, string name) =>
        {
            return context.Database.SqlQuery<Book>($"SELECT * FROM Books WHERE AuthorLastName = {name}");
        });
    }
}
