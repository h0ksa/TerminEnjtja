using Microsoft.EntityFrameworkCore;
using TerminEnjtja.Data;
using TerminEnjtja.Models;

namespace TerminEnjtja.Services
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _context;

        public MatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Match>> GetAllMatchesAsync()
        {
            return await _context.Matches
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        public async Task<Match> GetMatchByIdAsync(int id)
        {
            return await _context.Matches.FindAsync(id);
        }

        public async Task AddMatchAsync(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMatchAsync(Match match)
        {
            _context.Entry(match).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMatchAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
        }

        public (TeamStats teamA, TeamStats teamB) CalculateTeamStats(List<Match> matches)
        {
            var teamA = new TeamStats { TeamName = "ACI", Wins = 0, Draws = 0, Losses = 0 };
            var teamB = new TeamStats { TeamName = "BESI", Wins = 0, Draws = 0, Losses = 0 };

            foreach (var match in matches)
            {
                if (match.TeamAScore > match.TeamBScore)
                {
                    teamA.Wins++;
                    teamB.Losses++;
                }
                else if (match.TeamAScore < match.TeamBScore)
                {
                    teamB.Wins++;
                    teamA.Losses++;
                }
                else
                {
                    teamA.Draws++;
                    teamB.Draws++;
                }
            }

            return (teamA, teamB);
        }
    }
}
