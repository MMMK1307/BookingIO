namespace BookingIO.Models
{
    public class SpaceReservationModel
    {
        public SpaceReservationModel(Space space, List<Reserve> reservations)
        {
            Space = space;
            Reservations = reservations;
        }

        public Space Space { get; }
        public List<Reserve> Reservations { get; }
    }
}
