using System.Text.Json;
using BenchmarkDotNet.Attributes;
using FluentValidationApi.Entities;

namespace FluentValidation.Client.Beanchmark;
[MemoryDiagnoser]
public class FluentValidationBenchmark
{
    [Params(10)]
    public int size;
    [Benchmark]
    public async Task ConsumeCustomerEndPoint()
    {
        while (size > 0)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5214/api");

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

                await client.PostAsJsonAsync("customer", customerSerialized);
            }
                size--;
        }

    }

    [Benchmark]
    public async Task ConsumeCustomerAnnotaionEndPoint()
    {
        while (size > 0)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://localhost:5214/api");

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

                await client.PostAsJsonAsync("CustomerAnnotation", customerSerialized);
            }
            size--;
        }
    }

}
