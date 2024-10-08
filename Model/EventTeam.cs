namespace banbet.Models
{
    public class EventTeam
    {
        public int EventID { get; set; }
        public Event Event { get; set; }

        public int TeamID { get; set; }
        public Team Team { get; set; }
    }
}
