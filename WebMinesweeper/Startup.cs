using Games.Models;
using Microsoft.OpenApi.Models;

namespace Games.Web;

//конфигурация приложения
public class Startup
{
    public Startup()
    {    }

    //сервисы
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("MinesweeperV1", new OpenApiInfo { 
                Title = "Minesweeper.Api", 
                Version = "v1",
                Description = @"<p><a rel=""noopener noreferrer"" href="" / "">Игра Сапёр</a></p>"       
            });
        });
        //игровой центр
        services.AddSingleton<GamesProvider>();
    }

    //обработка запроса
    public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/MinesweeperV1/swagger.json", "Minesweeper v1"));
        }

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseEndpoints (endpoints => endpoints.MapControllers ());
    }
}
