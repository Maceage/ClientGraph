namespace ClientGraph.Domain
{
    public class Client : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelephoneNumber { get; set; }

        protected override string GetDescription()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
