using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AccountCheckService.Model;

namespace AccountCheckService.Actions
{
    public class ActionRequests
    {
        public static async Task<Account> GetAccount(string accnumber)
        {
            Account acc;
            HttpClient client = new HttpClient();
            string url = "http://host.docker.internal:3000/getBankAccount/";
            string query = "?accnumber=" + accnumber;
            HttpResponseMessage response = await client.GetAsync(url + query);
            if (response.IsSuccessStatusCode)
            {
                string resString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("response json "+resString);
                acc = JsonConvert.DeserializeObject<Account>(resString);
                Console.WriteLine("balance: "+acc.BankAccountAddress);
                return acc;
            }
            return null;
        }
    }
}
