namespace TerminEnjtja.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TeamAScore { get; set; } // ACI team score
        public int TeamBScore { get; set; } // BESI team score
        public string Notes { get; set; }
    }
}
