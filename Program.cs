using DrugiKlkPrimer.Data;
using DrugiKlkPrimer.Models;
using Microsoft.EntityFrameworkCore;

using var context = new BibliotekaContext();

context.Loans.RemoveRange(context.Loans);
context.Books.RemoveRange(context.Books);
context.Members.RemoveRange(context.Members);
context.Genres.RemoveRange(context.Genres);

context.SaveChanges();

var roman = new Genre
{
    Name = "Roman"
};

var naucnaFantastika = new Genre
{
    Name = "Naucna fanstastika"
};

context.Genres.AddRange(roman,naucnaFantastika);
context.SaveChanges();

var naDrini = new Book
{
    Title = "na drini cuprija",
    PublicationYear = 1945,
    Genre = roman
};
var b1984 = new Book
{
    Title = "1984",
    PublicationYear = 1949,
    Genre = naucnaFantastika
};

context.Books.AddRange(b1984, naDrini);
context.SaveChanges();

var marko = new Member
{
    FirstName = "marko",
    LastName = "Markovic"
};

context.Members.Add(marko);
context.SaveChanges();

var loan1 = new Loan
{
    Book = naDrini,
    Member = marko,
    NumberOfDays=14
};

var loan2 = new Loan
{
    Book = b1984,
    Member = marko,
    NumberOfDays = 21
};

context.Loans.AddRange(loan1, loan2);
context.SaveChanges();

Console.WriteLine("Podaci su uspešno sačuvani u bazi.");

var booksReport=context.Books
    .Include(b=>b.Genre)
    .Include(b=>b.Loans)
    .OrderBy(b=>b.Title)
    .Select(b=> new
    {
        BookTitle=b.Title,
        GenreName=b.Genre.Name,
        LoanCount=b.Loans.Count()
    })
    .ToList();

foreach (var book in booksReport)
{
    Console.WriteLine($"{book.BookTitle} Žanr: {book.GenreName} Broj pozajmica: {book.LoanCount}");

}

var bookTitleForExtension = "1984";
var memberFirstnameForExtension = "marko";
var memberLastNameForExtenstion = "Markovic";
var additionalDays = 7;

var loanToExtend = context.Loans
    .Include(l => l.Book)
    .Include(l => l.Member)
    .FirstOrDefault(l =>
        l.Book.Title.Equals(bookTitleForExtension) &&
        l.Member.FirstName.Equals(memberFirstnameForExtension) &&
        l.Member.LastName.Equals(memberLastNameForExtenstion)
    );

if (loanToExtend == null)
    Console.WriteLine("greska");


else
{
    Console.WriteLine($"stari broj dana:{loanToExtend.NumberOfDays}");
    loanToExtend.NumberOfDays += additionalDays;
    context.SaveChanges();
    var updatedLoan= context.Loans
    .Include(l => l.Book)
    .Include(l => l.Member)
    .FirstOrDefault(l =>
        l.Book.Title.Equals(bookTitleForExtension) &&
        l.Member.FirstName.Equals(memberFirstnameForExtension) &&
        l.Member.LastName.Equals(memberLastNameForExtenstion)
    );
    Console.WriteLine($"novi broj dana:{updatedLoan.NumberOfDays}");

}