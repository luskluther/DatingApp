using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using AutoMapper;

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
            
            // what we are doing here is askign the service to ignore these erros are we have user in phot and photo in user
            //Self referencing loop detected for property 'user' with type 'DatingApp.API.Models.User'. Path '[0].photos[0]'.
            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            // Ordering of services to the container is not that important
            // Odding cors as a service to the container here.
            services.AddCors(); 
            // adding automapper to the service contariner to user later.
            services.AddAutoMapper(typeof(DatingRepo).Assembly);
            // adding newtonsoft json suppoprt to the controllers here aftter installing package newtonsoft json
            // newtonsoft json will be replaced by a microsfot prackage .text as netwon is third party
            services.AddControllers().AddNewtonsoftJson();
            // Create one instance of a service or class and that can be used throughoyut the application best for concurrent requests
            //services.AddSingleton();
            // Lightweight stateless services , these are created each time the request is coming.
            //services.AddTransient();
            // Lervice is created once per scope. its in the middle of singleton and transient , suitable for our autrepo that we are creating here.
            services.AddScoped<IAuthRepository,AuthRepository>(); // This now is ready to be injected , has its interface and its concreate implementaion.
            services.AddScoped<IDatingRepo,DatingRepo>();
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
            } else {
                // Df we are dealing with a higher env we need to handle exceptions in a good way and this works as a global exception
                    // handler if some code doesnt have try catch
                    // the logic here could be complex but overall we are running appbuilder Iapplicationbuilder builder to exceptionhandler , running it
                    // then we are setting the reposne status code of the context
                    // then we are setting the error and storing
                    // if error is not null meaning ther is an error , we are adding the error message to the context reponse
                    // with this even without try catch and there are exceptions somewhere , it will send clean responses with information about exceptions
                    // overall the main objcetive is if its dev mode we will send developer exception default page. 
                    // if env is prod we get no information about the exception. that is what we are handling for some information about it with this handler.
                app.UseExceptionHandler(builder =>{
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null) {
                            context.Response.AddApplicationError(error.Error.Message); // this is the static class method that we are using , we are extending the reponse with this method
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
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
