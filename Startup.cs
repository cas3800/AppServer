using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace AppServer
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            AppConfiguration = config;
        }

        public static IConfiguration AppConfiguration { get; set; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            if (bool.Parse(AppConfiguration["AppServices:Init"])) app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/init", async context =>
                {
                    await context.Response.WriteAsync(await InitService.Init(context));
                });
            });
            if (bool.Parse(AppConfiguration["AppServices:Auth"])) app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/auth", async context =>
                {
                    await context.Response.WriteAsync(await AuthService.Auth(context));
                });
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/reg/", async context =>
                {
                    await context.Response.WriteAsync("Hello Reg!");
                });
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/profile/", async context =>
                {
                    //Проверить токен
                    await context.Response.WriteAsync("Hello Profile!");
                });
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/game/", async context =>
                {
                    //Проверить токен
                    await context.Response.WriteAsync("Hello GS!");
                });
            });
        }
    }
}
