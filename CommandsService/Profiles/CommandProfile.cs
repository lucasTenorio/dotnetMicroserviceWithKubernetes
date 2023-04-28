using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            //--> Source - Target
            CreateMap<Platform,PlatformReadDto>();
            CreateMap<CommandCreateDto,Command>();
            CreateMap<CommandReadDto,Command>();

            CreateMap<Command,CommandReadDto>();
            CreateMap<Command,CommandCreateDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember( destination => destination.ExternalId, options => options.MapFrom(source => source.Id))
                .ForMember( destination => destination.Id, options => options.Ignore());
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
        }
    }
}