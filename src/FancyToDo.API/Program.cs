using FancyToDo.API;
using FancyToDo.API.Configuration;
using FancyToDo.API.Middleware;
using FancyToDo.Infrastructure;
using SharedKernel.EventSourcing.EventStore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureDataStore();
builder.ConfigureEventStore(typeof(ToDoListEventStore).Assembly);


var app = builder.Build();
app.MapDefaultEndpoints();

app.UseLoggingMiddleware();

// await app.SeedTestData();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}