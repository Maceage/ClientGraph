using ClientGraph.Domain;
using ClientGraph.Models;
using ClientGraph.Services;

namespace ClientGraph.Controllers
{
    public class ClientController : EntityControllerBase<ClientModel, Client, ClientService>
    {
    }
}