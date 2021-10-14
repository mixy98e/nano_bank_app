using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WithdrawService.Model;
using WithdrawService.Actions;

namespace WithdrawService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WithdrawController : ControllerBase
    {

        [HttpGet("status")]
        public ActionResult GetServiceStatusAsync()
        {
            return Ok("Withdraw service is up and running");
        }

        [HttpPost]
        public IActionResult Post([FromBody] SingleTransactionData data)
        {
            Task<bool> task = ActionRequest.CheckAccountBalanceAsync(data);
            task.Wait();
            bool isValid = task.Result;
            Console.WriteLine(isValid);
            if (isValid)
                return ActionRequest.WithdrawAmountFromAccount(data).Result;
            return Ok("Account not valid for withdrawal");
        }
    }
}
