using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using FluentValidationApi.Entities;
namespace FluentValidationDemo.Client.Beanchmark;
[MemoryDiagnoser]
public class FluentValidationBenchmark
{
    [Params(10)]
    public int size;
    [Benchmark]
    public async Task ConsumeCustomerEndPoint()
    {

        using (var client = new HttpClient())
        {
            Uri uri = new Uri("http://localhost:5000/api/Customer");

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
            await client.PostAsync(uri, content);
        }

    }

    [Benchmark]
    public async Task ConsumeCustomerAnnotaionEndPoint()
    {
        using (var client = new HttpClient())
        {

            Uri uri = new Uri("http://localhost:5000/api/CustomerAnnotation");

            var customer = new CustomerAnnotation
            {
                Id = 1,
                Discount = 215m,
                Forename = "Islam",
                Surname = "Elnagar",
                Address = new AddressAnnotation
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
            await client.PostAsync(uri, content);
        }
    }

}
