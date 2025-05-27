using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.HttpClients;
using Secullum.Internationalization.WebService.Services;

namespace Secullum.Internationalization.WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<SecullumInternationalizationWebServiceContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SecullumInternationalizationWebServiceContext"))
            );

            services.Configure<TranslatorSettings>(Configuration.GetSection("TranslatorSettings"));

            services.AddHttpClient<ITranslationHttpClient, TranslationHttpClient>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<TranslatorSettings>>().Value;

                client.BaseAddress = new Uri(settings.Endpoint);

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", settings.SubscriptionKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", settings.Region);
            });

            services.AddScoped<TranslationService>();
            services.AddScoped<ExpressionsService>();

            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
