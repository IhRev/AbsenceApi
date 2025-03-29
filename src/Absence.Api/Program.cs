using Absence.Application;
using Absence.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();
builder.Services
    .AddApplication()
    .AddInfrastructure();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
//builder.Services.AddDbContext<AbsenceContext>(opt => opt.UseSqlServer(""));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = "http://absences/api",

            ValidateAudience = true,
            ValidAudience = "http://absences/api",

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SymmetricSecurityKey")),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Absence.Api");
        c.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowSpecificOrigin");
app.MapFallbackToFile("/index.html");
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();