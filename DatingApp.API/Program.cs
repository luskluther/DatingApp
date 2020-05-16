using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatingApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // all this is happening when application is starting
            var host = CreateHostBuilder(args).Build(); // creating the referenmce of host builder and removing to run()
            using(var scope = host.Services.CreateScope()){
                var services = scope.ServiceProvider;
                try { // To check if there any errors
                    var context = services.GetRequiredService<DataContext>();
                    context.Database.Migrate();  // everytyime we stat our application , it will cehck if there are any pending migrations it will do it
                    Seed.SeedUsers(context);
                    // Whats happening above is whever we start our application it not only creates the databsae, but it will also seed
                    // seed is nothing but loading the tables with values if db doesnt have any users etc
                } catch (Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "an error occured during migrations in program start");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
