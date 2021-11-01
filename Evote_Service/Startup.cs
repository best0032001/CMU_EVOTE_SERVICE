using Evote_Service.Model;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Repository;
using Evote_Service.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Evote_Service.Model.Repository.Mock;

namespace Evote_Service
{
    public class Startup
    {
        private IWebHostEnvironment webHostEnvironment;
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            webHostEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpClient();
            if (webHostEnvironment.IsEnvironment("test"))
            {
                services.AddDbContext<EvoteContext>(options => options.UseInMemoryDatabase(databaseName: "ApplicationDBContext").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
                services.AddScoped<ISMSRepository, SMSRepositoryMock>();
                services.AddScoped<IEmailRepository, EmailRepositoryMock>();
            }
            else {
                services.AddScoped<ISMSRepository, SMSRepositoryMock>();
                services.AddScoped<IEmailRepository, EmailRepository>();
            }
            services.AddDbContext<EvoteContext>(options => options.UseInMemoryDatabase(databaseName: "ApplicationDBContext").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            services.AddScoped<ICheckUserRepository, CheckUserRepository>();
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins(Environment.GetEnvironmentVariable("ORIGIN"))
                                               .AllowAnyMethod()
                                               .AllowAnyHeader()
                                               .AllowCredentials();
                                  });
            });
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new ITSCInputFormatter());
                foreach (var inputFormatter in options.InputFormatters.OfType<ITSCInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                }
            });

          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            SetData setData = new SetData(env);
            setData = null;
        }
    }
}
