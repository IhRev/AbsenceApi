﻿namespace Absence.Api;

public static class MiddlewareExtensions
{
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
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

        return app;
    }
}