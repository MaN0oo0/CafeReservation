namespace CafeReservationAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = "";
        public string Status { get; set; } = "بانتظار";

        public User? User { get; set; }
        public Table? Table { get; set; }
    }

}
