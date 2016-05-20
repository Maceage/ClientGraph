namespace ClientGraph.Domain
{
    public class Relationship
    {
        public EntityBase Parent { get; set; }
        public EntityBase Child { get; set; }
    }
}