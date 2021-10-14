using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentActionService.Model;
using PaymentActionService.Actions;

namespace PaymentActionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentActionController : ControllerBase
    {
        [HttpGet("status")]
        public ActionResult GetServiceStatus()
        {
            return Ok("PaymentAction service is up and running");
        }

        [HttpPost("/Single/{origin}")]
        public IActionResult Post([FromBody] SingleTransactionData data, string origin)
        {
            PaymentAction pa = new PaymentAction();
            pa.ActionType = origin;
            pa.SourceAddress = data.BankAccountAddress;
            pa.DestinationAddress = data.BankAccountAddress;
            pa.Amount = data.Amount;
            pa.Name = origin + ">" + data.BankAccountAddress;
            return ActionRequests.InsertPaymentAction(pa).Result;
        }

        [HttpPost("/Multi/{origin}")]
        public IActionResult Post([FromBody] MultiTransactionData data, string origin)
        {
            PaymentAction pa = new PaymentAction();
            pa.ActionType = origin;
            pa.SourceAddress = data.SourceAccountAddress;
            pa.DestinationAddress = data.DestinationAccountAddress;
            pa.Amount = data.Amount;
            pa.Name = origin + ">>" + data.SourceAccountAddress;
            return ActionRequests.InsertPaymentAction(pa).Result;
        }
    }
}
