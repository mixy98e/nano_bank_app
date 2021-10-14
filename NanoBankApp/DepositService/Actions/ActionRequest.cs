using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DepositService.Model;

namespace DepositService.Actions
{
    public class ActionRequest
    {

        public static async Task<bool> CheckAccountBalanceAsync(SingleTransactionData depositData)
        {
            if (depositData.BankAccountAddress != null && depositData.BankAccountAddress != "" && depositData.Amount > 0)
            {
                bool validity = false;
                HttpClient client = new HttpClient();
                string url = "http://accountcheckservice:80/";
                string query = depositData.BankAccountAddress + "/" + depositData.Amount + "/deposit-service";
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

        public static async Task<IActionResult> DepositAmountToAccount(SingleTransactionData depositData)
        {
            if (depositData.BankAccountAddress == null || depositData.BankAccountAddress == "" || depositData.Amount <= 0)
                return new StatusCodeResult(200);
            string url = "http://host.docker.internal:3000/"; 
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(depositData);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine("increase json "+content);
                try
                {
                    using (var response = await httpClient.PutAsync(url + "increaseAccountBalance", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        await PaymentActionRequest(depositData);
                        return new JsonResult(
                            new
                            {
                                resp = apiResponse,
                                message = "Deposit successfully executed",
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


        public static async Task<IActionResult> PaymentActionRequest(SingleTransactionData depositData)
        {
            string url = "http://paymentactionservice:80/";
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(depositData);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine("increase json " + content);
                try
                {
                    using (var response = await httpClient.PostAsync(url + "Single/deposit", content))
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
