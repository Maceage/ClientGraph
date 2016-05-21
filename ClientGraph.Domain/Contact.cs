namespace ClientGraph.Domain
{
    public class Contact : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }

        protected override string GetDescription()
        {
            return $"{FirstName} {LastName}";
        }
    }
}