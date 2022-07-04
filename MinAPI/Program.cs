using BookRepository.Core;
using BookRepository.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddScoped<IBookData, SqlData>();
builder.Services.AddDbContextPool<BookRepoDbContext>(dbContextOptns =>
{
    _ = dbContextOptns.UseSqlServer(builder.Configuration.GetConnectionString("BookConn"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/listbooks", async (IBookData service) =>
{
    return await service.ListBooksAsync();
})
.WithName("List Books");

app.MapPost("/book", async (Book book, IBookData service) =>
{
    _ = await service.SaveAsync(book);
})
.WithName("Add Book");

app.MapPut("/updatebook", async (Book book, IBookData service) =>
{
    _ = await service.UpdateAsync(book);
})
.WithName("Update Book");


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}