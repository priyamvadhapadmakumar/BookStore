using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository;
using BookStoreDataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //Mainly to configure dependency configuration
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            /*Because we chose individual user authentication, it automatically created this service where
             * if any user signs up, the application requires email confirmation*/
            /*Changed from addDefaultIdentity<IdentityUser> to 
             * AddIdentity<IdentityUser,IdentityRole> to include Roles
             */
            services.AddIdentity<IdentityUser,IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(); /*these will be added to our project as a part of
                                                            * depedency injection*/
            services.AddControllersWithViews();
            /*Incase of .NET 3.1, 
             * services.AddControllersWithViews().AddRazorRuntimeCompilation();to activate the feature
             *services.AddRazorPages();to add IdentityClassLibrary which are razor pages and we
             *need to add that manually here. But all these covered in .NET5.0 and above*/

            //To test Bookstore controller APIs
            services.AddSwaggerGen(s =>
            { 
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "Bookstore API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //add middlewares to project. Order of middleswares is very important. 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(); //because of this, we can use wwwroot folder files in app. Even images can be added

            app.UseRouting();

            //Because we selected individual user account authentication, these 2 automatically created
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //we can add as many types of routes as we need
                endpoints.MapControllerRoute( //for MVC pages - routing options
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");/*make sure of 
                             * not leaving any spaces before and after the = sign.*/ 
                endpoints.MapRazorPages(); //for Razor pages
            });

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(options =>
            {
                options.RouteTemplate = swaggerOptions.JsonRoute;
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
            });
        }
    }
}
