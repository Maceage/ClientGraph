using ClientGraph.Domain;
using ClientGraph.Models;
using ClientGraph.Services;

namespace ClientGraph.Controllers
{
    public class ContactController : ControllerBase<ContactModel, Contact, ContactService>
    {
    }
}