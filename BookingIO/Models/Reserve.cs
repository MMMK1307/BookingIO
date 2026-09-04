namespace BookingIO.Models
{
    public class Reserve
    {
        public Reserve(User user, string space, string status)
        {
            User = user;
            Space = space;
            Status = status;

        }
        public User User { get; set; }
        public string Space { get; set; }
        public string Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }




    }
}
