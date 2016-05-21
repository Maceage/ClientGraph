using System.Collections.Generic;
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
        public ActionResult Create(string submitType, RelationshipModel relationshipModel)
        {
            ViewBag.Clients = GetClients();
            ViewBag.Contacts = GetContacts();
            ViewBag.Practices = GetPractices();

            if (submitType == "Create")
            {
                if (ModelState.IsValid)
                {
                    EntityRelationship entityRelationship = Mapper.Map<EntityRelationship>(relationshipModel);

                    _relationshipService.CreateRelationship(entityRelationship);
                }
            }

            return View(relationshipModel);
        }

        private IList<ClientModel> GetClients()
        {
            IList<Client> clients = _clientService.GetAll();

            return Mapper.Map<IList<ClientModel>>(clients);
        }

        private IList<ContactModel> GetContacts()
        {
            IList<Contact> contacts = _contactService.GetAll();

            return Mapper.Map<IList<ContactModel>>(contacts);
        }

        private IList<PracticeModel> GetPractices()
        {
            IList<Practice> practices = _practiceService.GetAll();

            return Mapper.Map<IList<PracticeModel>>(practices);
        }
    }
}