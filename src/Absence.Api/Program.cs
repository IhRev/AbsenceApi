using Absence.Api;
using Absence.Application;
using Absence.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();

const string connectionStringName = "AbsenceDB";

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, connectionStringName)
    .AddApi();

var app = builder.Build();
app.AddMiddlewares();
await app.RunAsync();