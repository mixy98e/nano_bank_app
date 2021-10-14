using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepositService.Actions;
using DepositService.Model;

namespace DepositService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepositController : ControllerBase
    {
        
        [HttpGet("status")]
        public ActionResult GetServiceStatus()
        {
            return Ok("Deposit service is up and running");
        }

        [HttpPost]
        public IActionResult Post([FromBody] SingleTransactionData data)
        {
            Task<bool> task = ActionRequest.CheckAccountBalanceAsync(data);
            task.Wait();
            bool isValid = task.Result;
            Console.WriteLine(isValid);
            if (isValid)
                return ActionRequest.DepositAmountToAccount(data).Result;
            return Ok("Account not valid for deposit");
        }
    }
}
