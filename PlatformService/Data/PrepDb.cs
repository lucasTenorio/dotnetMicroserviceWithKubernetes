using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        private static bool _isProd;

        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            _isProd = isProd;
            using(var serviceScoped = app.ApplicationServices.CreateScope())
            {
                AppDbContext? context = serviceScoped.ServiceProvider.GetService<AppDbContext>();
                if(context != null)
                    SeedData(context);
            }
        }

        private static void SeedData(AppDbContext context)
        {
            if(_isProd)
            {
                Console.WriteLine(" --> Attempting apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($" --> Could not exectute Migrations {ex.Message}");
                }
            }
            if(!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data");
                context.Platforms.AddRange(
                    new Platform(){Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform(){Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform(){Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}