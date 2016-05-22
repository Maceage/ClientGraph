using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;

namespace ClientGraph.Mapping.Profiles
{
    public class PracticeProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Practice, PracticeModel>().ReverseMap();
        }
    }
}