using CommandService.Intefaces;
using CommandService.Models;

namespace CommandService.Data.Repositories
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context;
        }
        public void CreateCommand(int platformId, Command command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));
            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform plat)
        {
            if(plat == null)
                throw new ArgumentNullException(nameof(plat));
            _context.Platforms.Add(plat);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            var result = _context.Commands.First(c => c.PlatformId == platformId && c.Id == commandId);
            if(result == null) throw new ArgumentException("The Command does not exists with the passed paremeters");
            return result;
        }

        public IEnumerable<Command> GetCommandByPlatformId(int platformId)
        {
            var commands = _context.Commands
                    .Where(c => c.PlatformId == platformId &&
                                c.Platform != null);
            return commands.OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExits(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}