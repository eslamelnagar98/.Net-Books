using FluentValidationApi.Entities.Validator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDbContext<FluentValidationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))
    //.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
    .AddScoped<ICustomerRepository, CustomerRepository>();
    

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
