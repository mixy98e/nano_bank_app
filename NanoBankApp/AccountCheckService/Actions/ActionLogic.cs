using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountCheckService.Model;

namespace AccountCheckService.Actions
{
    public class ActionLogic
    {
        public static bool CheckAccount(Account acc, decimal amount, string origin)
        {
            if (origin == "withdraw-service")
            {
                if (acc.Status == "Active")
                {
                    if (acc.Balance - amount >= 0)
                        return true;
                }
                return false;
            }
            else
            {
                if (acc.Status == "Active")
                    return true;
                return false;
            }
        }
    }
}
