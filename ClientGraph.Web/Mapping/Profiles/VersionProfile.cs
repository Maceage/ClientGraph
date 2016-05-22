using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;

namespace ClientGraph.Mapping.Profiles
{
    public class VersionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<EntityVersion, VersionModel>();
            CreateMap<EntityVersion, VersionModel>().ReverseMap();
        }
    }
}