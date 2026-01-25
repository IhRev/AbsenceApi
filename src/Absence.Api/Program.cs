using Absence.Api;
using Absence.Application;
using Absence.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();
builder.Configuration.AddEnvironmentVariables();

const string connectionStringName = "AbsenceDB";

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, connectionStringName)
    .AddApi(builder.Configuration);

var app = builder.Build();
app.AddMiddlewares();
await app.RunAsync();