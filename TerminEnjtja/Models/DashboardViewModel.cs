using TerminEnjtja.Models;

public class DashboardViewModel
{
    public List<Match> Matches { get; set; }
    public TeamStats TeamAStats { get; set; } // ACI stats
    public TeamStats TeamBStats { get; set; } // BESI stats
    public bool IsAdmin { get; set; }
}