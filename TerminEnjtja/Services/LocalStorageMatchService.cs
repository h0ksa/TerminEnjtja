// Services/LocalStorageMatchService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TerminEnjtja.Models;

namespace TerminEnjtja.Services
{
    public class LocalStorageMatchService : IMatchService
    {
        private readonly string _dataFilePath;
        private readonly ILogger<LocalStorageMatchService> _logger;

        public LocalStorageMatchService(IWebHostEnvironment env, ILogger<LocalStorageMatchService> logger)
        {
            // Store data in a JSON file in the app's data directory
            var dataDirectory = Path.Combine(env.ContentRootPath, "App_Data");

            // Create directory if it doesn't exist
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            _dataFilePath = Path.Combine(dataDirectory, "matches.json");
            _logger = logger;

            // Create the file if it doesn't exist
            if (!File.Exists(_dataFilePath))
            {
                File.WriteAllText(_dataFilePath, JsonSerializer.Serialize(new List<Match>()));
            }
        }

        private async Task<List<Match>> ReadMatchesFromFileAsync()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return new List<Match>();
                }

                var json = await File.ReadAllTextAsync(_dataFilePath);
                return string.IsNullOrEmpty(json)
                    ? new List<Match>()
                    : JsonSerializer.Deserialize<List<Match>>(json) ?? new List<Match>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading matches from file");
                return new List<Match>();
            }
        }

        private async Task WriteMatchesToFileAsync(List<Match> matches)
        {
            try
            {
                var json = JsonSerializer.Serialize(matches, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing matches to file");
                throw;
            }
        }

        public async Task<List<Match>> GetAllMatchesAsync()
        {
            var matches = await ReadMatchesFromFileAsync();
            return matches.OrderByDescending(m => m.Date).ToList();
        }

        public async Task<Match> GetMatchByIdAsync(int id)
        {
            var matches = await ReadMatchesFromFileAsync();
            return matches.FirstOrDefault(m => m.Id == id);
        }

        public async Task AddMatchAsync(Match match)
        {
            var matches = await ReadMatchesFromFileAsync();

            // Generate a new ID (max ID + 1 or 1 if no matches)
            match.Id = matches.Count > 0 ? matches.Max(m => m.Id) + 1 : 1;

            matches.Add(match);
            await WriteMatchesToFileAsync(matches);
        }

        public async Task UpdateMatchAsync(Match match)
        {
            var matches = await ReadMatchesFromFileAsync();
            var existingMatch = matches.FirstOrDefault(m => m.Id == match.Id);

            if (existingMatch != null)
            {
                // Update properties
                existingMatch.Date = match.Date;
                existingMatch.TeamAScore = match.TeamAScore;
                existingMatch.TeamBScore = match.TeamBScore;
                existingMatch.Notes = match.Notes;

                await WriteMatchesToFileAsync(matches);
            }
        }

        public async Task DeleteMatchAsync(int id)
        {
            var matches = await ReadMatchesFromFileAsync();
            var matchToRemove = matches.FirstOrDefault(m => m.Id == id);

            if (matchToRemove != null)
            {
                matches.Remove(matchToRemove);
                await WriteMatchesToFileAsync(matches);
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