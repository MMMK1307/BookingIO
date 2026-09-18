namespace BookingIO.Models
{
    public class SpaceType
    {
        public SpaceType(string name)
        {
            Id = Guid.NewGuid();
            Name = name;           
        }

        private SpaceType() { }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
