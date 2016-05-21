using AutoMapper;
using ClientGraph.Mapping.Profiles;

namespace ClientGraph.Mapping
{
    public static class AutoMapperConfiguration
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ClientProfile>();
                cfg.AddProfile<ContactProfile>();
                cfg.AddProfile<PracticeProfile>();

                cfg.AddProfile<RelationshipProfile>();
            });
        }
    }
}