using PlatformService.Data;
using PlatformService.Interfaces;
using PlatformService.Models;

namespace PlatformService.Repositories
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context) => _context = context;
        public void CreatePlatform(Platform platform)
        {
            if(platform == null) throw new ArgumentException(nameof(platform));
            
            CheckDuplicates(platform.Id);

            _context.Add(platform);
            _context.SaveChanges();
        }
        private void CheckDuplicates(int id)
        {
            var plat = GetPlatformById(id);
            if(plat != null) throw new Exception("Platform already exists");
        }
        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return _context.Platforms.FirstOrDefault(platform => platform.Id == id);
        }

        public bool Update()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}