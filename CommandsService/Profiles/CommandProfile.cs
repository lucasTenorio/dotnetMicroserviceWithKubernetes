using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            //--> Source - Target
            CreateMap<Platform,PlatformReadDto>();
            CreateMap<CommandCreateDto,Command>();
            CreateMap<Command,CommandCreateDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember( destination => destination.ExternalId, options => options.MapFrom(source => source.Id));
        }
    }
}