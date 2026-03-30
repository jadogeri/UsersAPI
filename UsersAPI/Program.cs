using Microsoft.EntityFrameworkCore;
using UsersAPI.Data;
using UsersAPI.Repositories;
using UsersAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the DataSource (DatabaseContext)
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Data Access
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Business Logic
builder.Services.AddScoped<IUserService, UserService>(); 

var app = builder.Build();

// use for development
/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.EnsureCreated(); // This bypasses migrations and just creates the tables
}
*/


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
