using System.ComponentModel.DataAnnotations;

namespace TerminEnjtja.Models
{
    public class MatchViewModel
    {
        public DateTime Date { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Score must be a positive number")]
        public int TeamAScore { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Score must be a positive number")]
        public int TeamBScore { get; set; }
        public string Notes { get; set; }
    }
}