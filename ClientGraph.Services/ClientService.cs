using ClientGraph.Domain;

namespace ClientGraph.Services
{
    public class ClientService : EntityService<Client>
    {
        private const string PrimaryKey = "clientId";
        private const string TableName = "clients-graph-table";
        private const string BucketName = "clients";

        public ClientService()
            : base(PrimaryKey, TableName, BucketName)
        {
        }
    }
}
