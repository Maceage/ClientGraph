using ClientGraph.Domain;
using ClientGraph.Models;
using ClientGraph.Services;

namespace ClientGraph.Controllers
{
    public class ClientController : ControllerBase<ClientModel, Client, ClientService>
    {
    }
}