using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public static class ApplyMigrations
    {

        public static void apply(IApplicationBuilder app, bool _isProd)
        {
            if(!_isProd) return;

            using(var serviceScoped = app.ApplicationServices.CreateScope())
            {
                AppDbContext? context = serviceScoped.ServiceProvider.GetService<AppDbContext>();
                if(context != null)
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
                    
            }
        }

       
    }
}