using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataTableQueryBuilder.DataTables;

namespace SampleAPI
{
    using Core.Helpers;
    using Models;
    using Services;

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
            var connectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            // Add Entity Framework services to the services container.
            services.AddDbContext<DataContext>(o =>
            {
                o.UseMySql(connectionString, serverVersion);
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SampleAPI", Version = "v1" });
                c.CustomSchemaIds(type => type.ToString());
            });

            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            // Add application services.
            AddApplicationServices(services);

            services.AddDataTables();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, DevelopmentDataSeeder dataSeeder)
        {
            serviceProvider.GetService<IDataContext>()!.Database.Migrate();
            dataSeeder.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleAPI v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped<DevelopmentDataSeeder>();

            //Business Logic
            services.AddScoped<IDataContext, DataContext>(serviceProvider => serviceProvider.GetService<DataContext>()!);
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
