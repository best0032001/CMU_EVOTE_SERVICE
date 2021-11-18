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
            String origin = "";
            services.AddHttpClient();
            if (webHostEnvironment.IsEnvironment("test"))
            {
                services.AddDbContext<EvoteContext>(options => options.UseInMemoryDatabase(databaseName: "ApplicationDBContext").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
                services.AddScoped<ISMSRepository, SMSRepositoryMock>();
                services.AddScoped<IEmailRepository, EmailRepositoryMock>();
     
                origin = "*";
            }
            else {
                services.AddScoped<ISMSRepository, SMSRepository>();
                services.AddScoped<IEmailRepository, EmailRepository>();
                origin = Environment.GetEnvironmentVariable("ORIGIN");
            }
            services.AddDbContext<EvoteContext>(options => options.UseInMemoryDatabase(databaseName: "ApplicationDBContext").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            services.AddScoped<ICheckUserRepository, CheckUserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins(origin)
                                               .AllowAnyMethod()
                                               .AllowAnyHeader();
                                             
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EvoteContext evoteContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            env.IsProduction();
            app.UseRouting();

            app.UseAuthorization();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Use(async (context, next) =>
            {
                var url = context.Request.Path.Value;

                // Redirect to an external URL
                if (url.Contains("/api/login"))
                {
                    String redirect_uri = Environment.GetEnvironmentVariable("CMU_REDIRECT_URL"); 
                    String client_id = Environment.GetEnvironmentVariable("CMU_CLIENT_ID");
                    String oauth_scope = Environment.GetEnvironmentVariable("CMU_OAUTH_SCOPE");
                    String oauth_authorize_url = Environment.GetEnvironmentVariable("CMU_OAUTH_URL"); ;
                    String oauthUrl = "" + oauth_authorize_url + "?response_type=code&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&scope=" + oauth_scope;
                    context.Response.Redirect(oauthUrl);
                    return;   // short circuit
                }
                await next();
            });
            SetData setData = new SetData(env, evoteContext);
            setData = null;


        }
    }
}
