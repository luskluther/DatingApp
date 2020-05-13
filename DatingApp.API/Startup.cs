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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            // Create one instance of a service or class and that can be used throughoyut the application best for concurrent requests
            //services.AddSingleton();
            // Lightweight stateless services , these are created each time the request is coming.
            //services.AddTransient();
            // Lervice is created once per scope. its in the middle of singleton and transient , suitable for our autrepo that we are creating here.
            services.AddScoped<IAuthRepository,AuthRepository>(); // This now is ready to be injected , has its interface and its concreate implementaion.
            //Authentication as serivce
            // We are basically telling to use the token from the appsettings to use for jwt token reposne key which is used for authorization
            // By doing this api methods can only be consuned if the request is authorized
            // We can test this by saving the token after loging and using that in bearer token in the header while requesting or making the call
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
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

            // The steps and order that we are trying to configure for the request pipe line are important
            app.UseRouting();

            app.UseAuthentication();
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
