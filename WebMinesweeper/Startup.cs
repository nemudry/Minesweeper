using Games.Models;
using Microsoft.OpenApi.Models;

namespace Games.Web;

public class Startup
{
    public Startup()
    {    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { 
                Title = "Minesweeper.Api", 
                Version = "v1",
                Description = @"<p><a rel=""noopener noreferrer"" href="" / "">Игра Сапёр</a></p>"       
            });
        });
        services.AddSingleton<GamesProvider>();
    }

    public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minesweeper v1"));
        }

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints (endpoints =>  endpoints.MapControllers ());
    }
}
