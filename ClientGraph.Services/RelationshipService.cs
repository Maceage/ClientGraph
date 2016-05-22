using System;
using System.ComponentModel;
using System.Threading.Tasks;
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

        public async Task CreateRelationshipAsync(EntityRelationship entityRelationship)
        {
            EntityType parentEntityType = entityRelationship.ParentEntityType;
            EntityType childEntityType = entityRelationship.ChildEntityType;

            EntityBase parentEntityBase = await GetEntityAsync(parentEntityType, entityRelationship.ParentEntityId).ConfigureAwait(false);
            EntityBase childEntityBase = await GetEntityAsync(childEntityType, entityRelationship.ChildEntityId).ConfigureAwait(false);

            EntityNode parentNode = await CreateNodeAsync(parentEntityType, parentEntityBase).ConfigureAwait(false);
            EntityNode childNode = await CreateNodeAsync(childEntityType, childEntityBase).ConfigureAwait(false);

            using (GraphClient graphClient = CreateClient())
            {
                string relationshipTypeString = GetRelationshipTypeString(entityRelationship.RelationshipType);

                await graphClient.Cypher
                    .Match("(pe:" + parentEntityType + ")", "(ce:" + childEntityType + ")")
                    .Where((EntityNode pe) => pe.EntityId == parentNode.EntityId)
                    .AndWhere((EntityNode ce) => ce.EntityId == childNode.EntityId)
                    .CreateUnique("(pe)-[:" + relationshipTypeString + "]->(ce)")
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }
        }

        private static GraphClient CreateClient()
        {
            GraphClient graphClient = new GraphClient(new Uri(GrapheneEndpoint), GrapheneUsername, GraphenePassword);

            graphClient.JsonContractResolver = new CamelCasePropertyNamesContractResolver();
            graphClient.Connect();

            return graphClient;
        }

        private static async Task<EntityNode> CreateNodeAsync(EntityType entityType, EntityBase entity)
        {
            var entityNode = new EntityNode { EntityId = entity.Id, Name = entity.Name, Type = entityType };

            using (GraphClient graphClient = CreateClient())
            {
                await graphClient.Cypher
                    .Merge("(entity:" + entityType + " {entityId: {entityId} })")
                    .OnCreate()
                    .Set("entity = {newEntity}")
                    .WithParams(new { entityId = entityNode.EntityId, newEntity = entityNode })
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
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

        private async Task<EntityBase> GetEntityAsync(EntityType entityType, Guid entityId)
        {
            switch (entityType)
            {
                case EntityType.Client:
                    return await _clientService.GetByIdAsync(entityId).ConfigureAwait(false);
                case EntityType.Contact:
                    return await _contactService.GetByIdAsync(entityId).ConfigureAwait(false);
                case EntityType.Practice:
                    return await _practiceService.GetByIdAsync(entityId).ConfigureAwait(false);
                default:
                    throw new InvalidEnumArgumentException(nameof(entityType));
            }
        }
    }
}