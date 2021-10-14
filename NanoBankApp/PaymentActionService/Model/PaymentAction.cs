using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentActionService.Model
{
    public class PaymentAction
    {
        public string ActionType { get; set; }
        public decimal Amount { get; set; }
        public string DestinationAddress { get; set; }
        public string SourceAddress { get; set; }
        public string Name { get; set; }
    }
}
