using Domain.Users;
using Infrastructure.Persistence;
using Infrastructure.Users;
using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "appdb");

builder.Services.AddTransient<IUserDbAction, UserDbAction>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.UseStatusCodePages();

app.Run();