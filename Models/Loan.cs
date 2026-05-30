using System;
using System.Collections.Generic;
using System.Text;

namespace DrugiKlkPrimer.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public Book Book { get; set; }
        public int NumberOfDays { get; set; }
    }
}
