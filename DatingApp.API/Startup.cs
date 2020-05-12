using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // This can also be called as a dependency injection container if not easily understandable
        public void ConfigureServices(IServiceCollection services)
        {
            // We are telling the container wehave a service called db context serivce which is the data context class 
                // which has some setttings for establishing connections stuff like that
            services.AddDbContext<DataContext>(x => 
                                        x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            // Ordering of services to the container is not that important
            // Odding cors as a service to the container here.
            services.AddCors(); 
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // This method is used to configure the HTTP call request pipeline
        // Everythign here is middleware where the regquest can be impacted as it goes through the steps
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Developer friendly exception common page
                app.UseDeveloperExceptionPage();
            }

            // Return HTTP requests to HTTPS
            // app.UseHttpsRedirection();

            // The steps that we are trying to configure for the request pipe line are important
            app.UseRouting();

            app.UseAuthorization();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseEndpoints(endpoints =>
            {
                // Controller End Points are mapped here
                endpoints.MapControllers();
            });
        }
    }
}
