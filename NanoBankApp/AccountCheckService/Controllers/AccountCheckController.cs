using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountCheckService.Actions;
using AccountCheckService.Model;

namespace AccountCheckService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountCheckController : ControllerBase
    {

        [HttpGet("status")]
        public ActionResult GetServiceStatus()
        {
            return Ok("AccountCheck is up and running.");
        }

        [HttpGet("/{accnumber}/{accamount}/{origin}")]
        public ActionResult GetServiceStatus(string accnumber, decimal accamount, string origin)
        {
            Console.WriteLine(accnumber+"/"+ accamount);
            Task<Account> task = ActionRequests.GetAccount(accnumber);
            task.Wait();
            Account acc = task.Result;
            return Ok(ActionLogic.CheckAccount(acc, accamount, origin));
        }

    }
}
