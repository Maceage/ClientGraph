using System;
using System.ComponentModel;
using ClientGraph.Domain;
using ClientGraph.Domain.Enumerations;
using Neo4jClient;
using Newtonsoft.Json.Serialization;

namespace ClientGraph.Services
{
    public class RelationshipService
    {
        private const string GrapheneEndpoint = "http://clientgraph.sb05.stations.graphenedb.com:24789/db/data/";
        private const string GrapheneUsername = "ClientGraph";
        private const string GraphenePassword = "ety7XOVerFVBwufpWLIp";

        private readonly ClientService _clientService;
        private readonly ContactService _contactService;
        private readonly PracticeService _practiceService;

        public RelationshipService()
            : this(new ClientService(), new ContactService(), new PracticeService())
        {
        }

        public RelationshipService(ClientService clientService, ContactService contactService, PracticeService practiceService)
        {
            _clientService = clientService;
            _contactService = contactService;
            _practiceService = practiceService;
        }

        public void CreateRelationship(EntityRelationship entityRelationship)
        {
            EntityType parentEntityType = entityRelationship.ParentEntityType;
            EntityType childEntityType = entityRelationship.ChildEntityType;

            EntityBase parentEntityBase = GetEntity(parentEntityType, entityRelationship.ParentEntityId);
            EntityBase childEntityBase = GetEntity(childEntityType, entityRelationship.ChildEntityId);

            EntityNode parentNode = CreateNode(parentEntityType, parentEntityBase);
            EntityNode childNode = CreateNode(childEntityType, childEntityBase);

            using (GraphClient graphClient = CreateClient())
            {
                string relationshipTypeString = GetRelationshipTypeString(entityRelationship.RelationshipType);

                 graphClient.Cypher
                    .Match("(pe:" + parentEntityType + ")", "(ce:" + childEntityType + ")")
                    .Where((EntityNode pe) => pe.EntityId == parentNode.EntityId)
                    .AndWhere((EntityNode ce) => ce.EntityId == childNode.EntityId)
                    .CreateUnique("(ce)-[:" + relationshipTypeString + "]->(pe)")
                    .ExecuteWithoutResults();
            }
        }

        public void DeleteRelationship(EntityBase parent, EntityBase child)
        {
        }

        private static GraphClient CreateClient()
        {
            GraphClient graphClient = new GraphClient(new Uri(GrapheneEndpoint), GrapheneUsername, GraphenePassword);

            graphClient.JsonContractResolver = new CamelCasePropertyNamesContractResolver();
            graphClient.Connect();

            return graphClient;
        }

        private static EntityNode CreateNode(EntityType entityType, EntityBase entity)
        {
            var entityNode = new EntityNode { EntityId = entity.Id, Name = entity.Name, Type = entityType };

            using (GraphClient graphClient = CreateClient())
            {
                graphClient.Cypher
                    .Merge("(entity:" + entityType + " {entityId: {entityId} })")
                    .OnCreate()
                    .Set("entity = {newEntity}")
                    .WithParams(new { entityId = entityNode.EntityId, newEntity = entityNode })
                    .ExecuteWithoutResults();
            }

            return entityNode;
        }

        private static string GetRelationshipTypeString(RelationshipType relationshipType)
        {
            switch (relationshipType)
            {
                case RelationshipType.ClientOf:
                    return "CLIENT_OF";
                case RelationshipType.ContactOf:
                    return "CONTACT_OF";
                case RelationshipType.BusinessWith:
                    return "BUSINESS_WITH";
                default:
                    throw new InvalidEnumArgumentException(nameof(relationshipType));
            }
        }

        private EntityBase GetEntity(EntityType entityType, Guid entityId)
        {
            switch (entityType)
            {
                case EntityType.Client:
                    return _clientService.GetById(entityId);
                case EntityType.Contact:
                    return _contactService.GetById(entityId);
                case EntityType.Practice:
                    return _practiceService.GetById(entityId);
                default:
                    throw new InvalidEnumArgumentException(nameof(entityType));
            }
        }
    }
}