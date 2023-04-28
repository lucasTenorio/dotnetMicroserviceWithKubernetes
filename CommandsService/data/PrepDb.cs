using CommandService.Intefaces;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
            }

        }
        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms" );
            foreach (var plat in platforms)
            {
                if(!repository.ExternalPlatformExists(plat.ExternalId))
                    repository.CreatePlatform(plat);
                repository.SaveChanges();
            }
        }
    }
}