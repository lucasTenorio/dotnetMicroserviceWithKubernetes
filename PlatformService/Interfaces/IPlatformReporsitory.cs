using PlatformService.Models;

namespace PlatformService.Interfaces
{
    public interface IPlatformRepository
    {
        bool Update();
        IEnumerable<Platform> GetAllPlatforms();
        Platform GetPlatformById(int id);
        void CreatePlatform(Platform platform);
    }
}