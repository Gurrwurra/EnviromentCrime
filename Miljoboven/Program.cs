using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using EnviromentCrime.Models;


namespace Miljoboven
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateWebHostBuilder(args).Build();

      InitializeDatabase(host);
      InitializeIdentity(host);
      host.Run();
    }

    private static void InitializeDatabase(IWebHost host)
    {
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          DBInitializer.PopulateDB(services);
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while seeding the database.");
        }
      }
    }

    private static void InitializeIdentity(IWebHost host)
    {
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          DBIdentityInitializer.PopulateDB(services).Wait();
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while seeding the database.");
        }
      }
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
