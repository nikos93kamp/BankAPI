using BankAPI.DAL;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;
using BankAPI.Utils;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// we will register our database context to the built-in IOC container
builder.Services.AddDbContext<BankDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BankDbConnection")));
// configure our dependencies in our DI container
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Banking API Doc",
        Version = "v2",
        Description = "A Restful Bank WebAPI Service",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Nikos",
            Email = "nikos@gmail.com",
            Url = new Uri("https://github.com/nikos")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        var prefix = string.IsNullOrEmpty(x.RoutePrefix) ? "." : "..";
        x.SwaggerEndpoint($"{prefix}/swagger/v2/swagger.json", "Banking API Doc");
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
