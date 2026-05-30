using System;
using System.Collections.Generic;
using System.Text;

namespace DrugiKlkPrimer.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Loan> Loans { get; set; }


    }
}
