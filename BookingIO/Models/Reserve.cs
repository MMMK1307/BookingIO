namespace BookingIO.Models
{
    public enum ReserveStatus
    {
        Released,
        Blocked,
        Canceled,
        Rejected,

        Reserved
    }



    public class Reserve
    {
        public Reserve(User user, Space space, ReserveStatus status, DateTime start, DateTime end)
        {
            Id = Guid.NewGuid();
            User = user;
            Space = space;
            Status = status.ToString();
            Start = start;
            End = end;
        }

        private Reserve() { }

        public Guid Id { get; set; }
        public User User { get; set; }
        public Space Space { get; set; }
        public string Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
