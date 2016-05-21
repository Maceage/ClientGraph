using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;

namespace ClientGraph.Mapping.Profiles
{
    public class RelationshipProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<EntityRelationship, RelationshipModel>();
            CreateMap<EntityRelationship, RelationshipModel>().ReverseMap();
        }
    }
}