using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;


namespace belt2
{

   
    public class Startup
    {
         public IConfiguration Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DojoActivityCenterContext>(options => options.UseMySQL(Configuration["DBInfo:ConnectionString"]));
            // Add framework services.
            services.AddMvc();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {

            if( true )
            {
                loggerFactory.AddConsole();
                app.UseDeveloperExceptionPage();
                Console.WriteLine("We are in developmet");
            }          
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
        }
    }
}
