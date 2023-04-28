using CommandService.Models;

namespace CommandService.Intefaces
{
    public interface ICommandRepository{
        bool SaveChanges();
        //Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExists(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);
        
        //Commands
        IEnumerable<Command> GetCommandByPlatformId(int platformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}