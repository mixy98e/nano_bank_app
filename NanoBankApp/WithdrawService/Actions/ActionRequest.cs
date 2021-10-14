using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WithdrawService.Model;

namespace WithdrawService.Actions
{
    public class ActionRequest
    {

        public static async Task<bool> CheckAccountBalanceAsync(SingleTransactionData withdrawData)
        {
            if (withdrawData.BankAccountAddress != null && withdrawData.BankAccountAddress != "" && withdrawData.Amount > 0)
            {
                bool validity = false;
                HttpClient client = new HttpClient();
                string url = "http://accountcheckservice:80/";
                string query = withdrawData.BankAccountAddress + "/" + withdrawData.Amount + "/withdraw-service";
                HttpResponseMessage response = await client.GetAsync(url+query);
                if (response.IsSuccessStatusCode)
                {
                    string resString = await response.Content.ReadAsStringAsync();
                    validity = JsonConvert.DeserializeObject<bool>(resString);
                }
                return validity;
            }
            return false;
        }

        public static async Task<IActionResult> WithdrawAmountFromAccount(SingleTransactionData withdrawData)
        {
            if (withdrawData.BankAccountAddress == null || withdrawData.BankAccountAddress == "" || withdrawData.Amount <= 0)
                return new StatusCodeResult(200);
            // ovo promeniti u internal url kada se doda nodejs u docker compose
            string url = "http://host.docker.internal:3000/"; 
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(withdrawData);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine("decrease json "+content);
                try
                {
                    using (var response = await httpClient.PutAsync(url + "decreaseAccountBalance", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        await PaymentActionRequest(withdrawData);
                        return new JsonResult(
                            new
                            {
                                resp = apiResponse,
                                message = "Wthdraw successfully executed",
                            }
                        );
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                return new StatusCodeResult(400);
            }
        }


        public static async Task<IActionResult> PaymentActionRequest(SingleTransactionData withdrawData)
        {
            string url = "http://paymentactionservice:80/";
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(withdrawData);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                try
                {
                    using (var response = await httpClient.PostAsync(url + "Single/withdraw", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return new JsonResult(
                            new
                            {
                                resp = apiResponse,
                                message = "PaymentRequest successfully executed",
                            }
                        );
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                return new StatusCodeResult(400);
            }
        }
    }
}
