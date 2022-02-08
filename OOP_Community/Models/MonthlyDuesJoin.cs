using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OOP_Community.Models
{
    public class MonthlyDuesJoin
    {
        public Resident ResidentList { get; set; }
        public User UserList { get; set; }
        public Transaction TransactionList { get; set; }
        public Monthly_Due MonthlyDueList { get; set; }
    }
}