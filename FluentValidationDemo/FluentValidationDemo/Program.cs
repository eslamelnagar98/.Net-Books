using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidationDemo.Entities;
using FluentValidationDemo.Entities.Validator;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
CheckCustomerValidations();
app.MapGet("/", () => "Hello World!");
app.Run();
static void CheckCustomerValidations()
{
    var customer = new Customer();
    var validator = new CustomerValidator();
    validator.ValidateAndThrow(customer);
}