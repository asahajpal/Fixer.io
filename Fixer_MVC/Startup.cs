using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ATGCustReg_MVC.DataModel;
using ATGCustReg_MVC.WebServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ATGCustReg_MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();
            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });

            //services.AddControllers();

            services.AddControllersWithViews();
            services.AddMvcCore();
            //services.AddMvc();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "FixerApi", Version = "v1" });
            });

            // inject ServicePoints from appSettings.json
            var servicePoints = new FixerServiceSettings();
            Configuration.GetSection(FixerServiceSettings.ServiceSettings).Bind(servicePoints);  // binding here

            services.AddScoped<IFixerServiceSettings, FixerServiceSettings>(fsc => servicePoints);

            // injecting (or registering) DbConext ie. ExchangeRateContext here to make it available on demand
            services.AddDbContext<ExchangeRateContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // injecting FixerServiceClient here via DI so that it is available on demand
            services.AddHttpClient<IFixerServiceClient, FixerServiceClient>(servicePoints.ApiTag,
                client =>
                {
                    client.BaseAddress = new Uri(servicePoints.BaseUrl);
                });

            //The AddDatabaseDeveloperPageExceptionFilter provides helpful error information in the development environment
            services.AddDatabaseDeveloperPageExceptionFilter();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                //app.UseSwaggerUI();
                
                app.UseSwaggerUI(config =>
                {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "FixerApi");
                });
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            }); 
            
            
            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            */
        }
    }

    public interface IFixerServiceSettings
    {
        string ApiTag { get; set; }
        string AccessKey { get; set; }
        string BaseUrl { get; set; }
        string EndPoint { get; set; }
    }

    public class FixerServiceSettings : IFixerServiceSettings
    {
        public const string ServiceSettings = "ServiceSettings";
        public string ApiTag { get; set; }

        public string AccessKey { get; set; }

        public string BaseUrl { get; set; }

        public string EndPoint { get; set; }
    }
}
