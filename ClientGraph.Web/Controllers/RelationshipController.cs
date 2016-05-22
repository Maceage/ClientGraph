using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;
using ClientGraph.Services;

namespace ClientGraph.Controllers
{
    public class RelationshipController : Controller
    {
        private readonly RelationshipService _relationshipService;

        private readonly ClientService _clientService;
        private readonly ContactService _contactService;
        private readonly PracticeService _practiceService;

        public RelationshipController()
        {
            _relationshipService = new RelationshipService();

            _clientService = new ClientService();
            _contactService = new ContactService();
            _practiceService = new PracticeService();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ViewResult> Create(string submitType, RelationshipModel relationshipModel)
        {
            ViewBag.Clients = await GetClientsAsync().ConfigureAwait(false);
            ViewBag.Contacts = await GetContactsAsync().ConfigureAwait(false);
            ViewBag.Practices = await GetPracticesAsync().ConfigureAwait(false);

            if (submitType == "Create")
            {
                if (ModelState.IsValid)
                {
                    EntityRelationship entityRelationship = Mapper.Map<EntityRelationship>(relationshipModel);

                    await _relationshipService.CreateRelationshipAsync(entityRelationship).ConfigureAwait(false);
                }
            }

            return View(relationshipModel);
        }

        private async Task<IList<ClientModel>> GetClientsAsync()
        {
            IList<Client> clients = await _clientService.GetAllAsync().ConfigureAwait(false);

            return Mapper.Map<IList<ClientModel>>(clients);
        }

        private async Task<IList<ContactModel>> GetContactsAsync()
        {
            IList<Contact> contacts = await _contactService.GetAllAsync().ConfigureAwait(false);

            return Mapper.Map<IList<ContactModel>>(contacts);
        }

        private async Task<IList<PracticeModel>> GetPracticesAsync()
        {
            IList<Practice> practices = await _practiceService.GetAllAsync().ConfigureAwait(false);

            return Mapper.Map<IList<PracticeModel>>(practices);
        }
    }
}