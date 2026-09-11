namespace BookingIO.Models
{
    public class Reserve
    {
        public Reserve(User user, Space space, string status, DateTime start, DateTime end)
        {
            Id = Guid.NewGuid();
            User = user;
            Space = space;
            Status = status;
            Start = start;
            End = end;
        }
        public Guid Id { get; set; }
        public User User { get; set; }
        public Space Space { get; set; }
        public string Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }




    }
}
