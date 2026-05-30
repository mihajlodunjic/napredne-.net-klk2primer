using System;
using System.Collections.Generic;
using System.Text;

namespace DrugiKlkPrimer.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<Loan> Loans { get; set; }

    }
}
