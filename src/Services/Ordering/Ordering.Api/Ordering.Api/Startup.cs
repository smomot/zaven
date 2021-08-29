using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Ordering.Api.Infrastructure;
using Ordering.Api.Model;
using Ordering.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Api
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
            ConfigureDatabse(services);
            ConfigureDataAccessLayer(services);
            services.AddControllers().AddDapr(); ;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.Api", Version = "v1" });
            });
        }


        public void ConfigureDataAccessLayer(IServiceCollection services)
        {
            services.AddScoped<IOrderingRepository<OrderingEntity>, OrderingRepository<OrderingEntity>>();
           
        }

        public void ConfigureDatabse(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(
                Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings, DatabaseSettings>(sp =>
             sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddScoped<IOrderingContext<OrderingEntity>, OrderingContext<OrderingEntity>>();
          
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
