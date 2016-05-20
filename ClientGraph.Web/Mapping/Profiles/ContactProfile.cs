using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;

namespace ClientGraph.Mapping.Profiles
{
    public class ContactProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Contact, ContactModel>();
            CreateMap<Contact, ContactModel>().ReverseMap();
        }
    }
}