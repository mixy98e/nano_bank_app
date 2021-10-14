using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WithdrawService.Model
{
    public class SingleTransactionData
    {
        public string BankAccountAddress { get; set; }
        public decimal Amount { get; set; }
    }
}
