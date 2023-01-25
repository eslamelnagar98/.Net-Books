//using BenchmarkDotNet.Running;
//using FluentValidation.Client.Beanchmark;

//BenchmarkRunner.Run<FluentValidationBenchmark>();
using System.Text;
using System.Text.Json;
using FluentValidationApi.Entities;
await ConsumeCustomerEndPoint();
static async Task ConsumeCustomerEndPoint()
{
    using var client = new HttpClient();

    Uri uri = new Uri("http://localhost:7048/api/Customer");

    var customer = new Customer
    {
        Id = 1,
        Discount = 215m,
        Forename = "Islam",
        Surname = "Elnagar",
        Address = new Address
        {
            Line1 = "Cairo",
            Line2 = "Cairo",
            County = "Egypt",
            Postcode = "12484",
            Town = "Giza"
        }
    };

    var customerSerialized = JsonSerializer.Serialize(customer);
    HttpContent content = new StringContent(customerSerialized, Encoding.UTF8, "application/json");
    var responseTask = await client.PostAsync(uri, content);
    //responseTask.Wait();

    var result = responseTask;
    if (result.IsSuccessStatusCode)
    {
        Console.WriteLine("Success");
    }

}