using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentActionService.Model
{
    public class MultiTransactionData
    {
        public string SourceAccountAddress { get; set; }
        public string DestinationAccountAddress { get; set; }
        public decimal Amount { get; set; }
    }
}
