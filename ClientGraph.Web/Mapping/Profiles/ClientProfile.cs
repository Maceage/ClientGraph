using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;

namespace ClientGraph.Mapping.Profiles
{
    public class ClientProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Client, ClientModel>().ReverseMap();
        }
    }
}