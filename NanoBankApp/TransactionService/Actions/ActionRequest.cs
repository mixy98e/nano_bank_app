using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Model;

namespace TransactionService.Actions
{
    public class ActionRequest
    {

        public static async Task<bool> CheckAccountBalanceAsync(MultiTransactionData transaction, string addrType)
        {
            if (transaction.SourceAccountAddress != null &&
                transaction.SourceAccountAddress != "" &&
                transaction.DestinationAccountAddress != null &&
                transaction.DestinationAccountAddress != "" &&
                transaction.Amount > 0)
            {
                bool validity = false;

                string tempAddress;
                string tempOrigin;
                if (addrType == "source")
                {
                    tempAddress = transaction.SourceAccountAddress;
                    tempOrigin = "withdraw-service";
                }
                else
                {
                    tempAddress = transaction.DestinationAccountAddress;
                    tempOrigin = "deposit-service";
                }
                HttpClient client = new HttpClient();
                string url = "http://accountcheckservice:80/";
                string query = tempAddress + "/" + transaction.Amount + "/" + tempOrigin;
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

        public static async Task<IActionResult> TransactionAmountFromToAccount(MultiTransactionData transaction)
        {
            if (transaction.SourceAccountAddress == null &&
               transaction.SourceAccountAddress == "" &&
               transaction.DestinationAccountAddress == null &&
               transaction.DestinationAccountAddress == "" &&
               transaction.Amount <= 0)
            {
                return new StatusCodeResult(200);
            }
            string url = "http://host.docker.internal:3000/"; 
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(transaction);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine("transact json "+content);
                try
                {
                    using (var response = await httpClient.PutAsync(url + "transactAmount", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        await PaymentActionRequest(transaction);
                        return new JsonResult(
                            new
                            {
                                resp = apiResponse,
                                message = "transaction successfully executed",
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

        public static async Task<IActionResult> PaymentActionRequest(MultiTransactionData depositData)
        {
            string url = "http://paymentactionservice:80/";
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(depositData);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine("transact json " + content);
                try
                {
                    using (var response = await httpClient.PostAsync(url + "Multi/transaction", content))
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
