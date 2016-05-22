using ClientGraph.Domain;

namespace ClientGraph.Services
{
    public class PracticeService : EntityService<Practice>
    {
        private const string PrimaryKey = "practiceId";
        private const string TableName = "practices-graph-table";
        private const string FolderName = "practices";

        public PracticeService()
            : base(PrimaryKey, TableName, FolderName)
        {
        }
    }
}