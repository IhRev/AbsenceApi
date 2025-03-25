using AbsenceApi.Database.Contexts;
using AbsenceApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<AbsenceMockService>();
builder.Services.AddDbContext<AbsenceContext>(opt => opt.UseSqlServer(""));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", nameof(AbsenceApi));
        c.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowSpecificOrigin");
app.MapFallbackToFile("/index.html");
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();