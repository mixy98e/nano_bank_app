using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionService.Model;
using TransactionService.Actions;

namespace TransactionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        [HttpGet("status")]
        public ActionResult GetServiceStatus()
        {
            return Ok("Transaction service is up and running");
        }

        [HttpPost]
        public IActionResult Post([FromBody] MultiTransactionData data)
        {
            Task<bool> task = ActionRequest.CheckAccountBalanceAsync(data, "source");
            task.Wait();
            bool isValidSrc = task.Result;
            task = ActionRequest.CheckAccountBalanceAsync(data, "destination");
            bool isValidDest = task.Result;
            Console.WriteLine("src: " + isValidSrc + ", dest:" + isValidDest);
            task.Wait();
            if (isValidSrc && isValidDest)
                return ActionRequest.TransactionAmountFromToAccount(data).Result;
            return Ok("Accounts are not valid for transaction");
        }
    }
}
