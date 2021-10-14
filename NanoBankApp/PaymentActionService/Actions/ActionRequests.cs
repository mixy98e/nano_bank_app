using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PaymentActionService.Model;

namespace PaymentActionService.Actions
{
    public class ActionRequests
    {
        public static async Task<IActionResult> InsertPaymentAction(PaymentAction data)
        {
            string url = "http://host.docker.internal:3000/";
            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine("decrease json " + content);
                try
                {
                    using (var response = await httpClient.PostAsync(url + "paymentAction", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return new JsonResult(
                            new
                            {
                                resp = apiResponse,
                                message = "PaymentAction successfully created and inserted",
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
