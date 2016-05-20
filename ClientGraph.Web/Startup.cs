using ClientGraph.Mapping;
using Owin;

namespace ClientGraph
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutoMapperConfiguration.Initialize();
        }
    }
}