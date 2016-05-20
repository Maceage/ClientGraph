namespace ClientGraph.Domain
{
    public class Practice : EntityBase
    {
        public string CompanyName { get; set; } 
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; }

        public override string GetDescription()
        {
            return CompanyName;
        }
    }
}