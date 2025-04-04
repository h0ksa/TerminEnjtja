using TerminEnjtja.Models;

namespace TerminEnjtja.Services
{
    public interface IMatchService
    {
        Task<List<Match>> GetAllMatchesAsync();
        Task<Match> GetMatchByIdAsync(int id);
        Task AddMatchAsync(Match match);
        Task UpdateMatchAsync(Match match);
        Task DeleteMatchAsync(int id);
        (TeamStats teamA, TeamStats teamB) CalculateTeamStats(List<Match> matches);
    }
}
