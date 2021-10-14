using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCheckService.Model
{
    public class Account
    {
        public String Name { get; set; }
        public String AccountOwner { get; set; }
        public String BankAccountAddress { get; set; }
        public Decimal Balance { get; set; }
        public String Status { get; set; }
    }
}
