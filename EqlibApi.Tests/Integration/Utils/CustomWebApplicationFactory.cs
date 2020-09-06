using EqlibApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EqlibApi.Tests.Integration.Utils
{
    class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));
                services.Remove(descriptor);

                services.AddDbContext<ApplicationContext>(opts =>
                {
                    opts.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationContext>();

                    db.Database.EnsureCreated();
                }
            });



        }
    }
}
