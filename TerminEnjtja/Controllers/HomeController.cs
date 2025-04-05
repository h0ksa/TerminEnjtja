using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TerminEnjtja.Models;
using TerminEnjtja.Services;

namespace TerminEnjtja.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly ILogger<HomeController> _logger;
        private const string ViewerPassword = "97";
        private const string AdminPassword = "besi1234";

        public HomeController(IMatchService matchService, ILogger<HomeController> logger)
        {
            _matchService = matchService ?? throw new ArgumentNullException(nameof(matchService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Clear any existing session when showing login page
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                if (model.Password == AdminPassword)
                {
                    HttpContext.Session.SetString("LoginType", "admin");
                    _logger.LogInformation("Admin logged in");
                    return RedirectToAction(nameof(Dashboard));
                }

                if (model.Password == ViewerPassword)
                {
                    HttpContext.Session.SetString("LoginType", "viewer");
                    _logger.LogInformation("Viewer logged in");
                    return RedirectToAction(nameof(Dashboard));
                }

                ModelState.AddModelError("Password", "Incorrect password! Please try again.");
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View("Index", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var loginType = HttpContext.Session.GetString("LoginType");
                if (string.IsNullOrEmpty(loginType))
                {
                    return RedirectToAction(nameof(Index));
                }

                var matches = await _matchService.GetAllMatchesAsync();
                var (teamAStats, teamBStats) = _matchService.CalculateTeamStats(matches);

                var viewModel = new DashboardViewModel
                {
                    Matches = matches,
                    TeamAStats = teamAStats,
                    TeamBStats = teamBStats,
                    IsAdmin = loginType == "admin"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return RedirectToAction(nameof(Error));
            }
        }

        [HttpGet]
        public IActionResult AddMatch()
        {
            if (HttpContext.Session.GetString("LoginType") != "admin")
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var model = new MatchViewModel
            {
                Date = GetNextThursday()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMatch(MatchViewModel model)
        {
            if (HttpContext.Session.GetString("LoginType") != "admin")
            {
                return RedirectToAction(nameof(Dashboard));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var match = new Match
                {
                    Date = model.Date,
                    TeamAScore = model.TeamAScore,
                    TeamBScore = model.TeamBScore,
                    Notes = model.Notes
                };

                await _matchService.AddMatchAsync(match);
                _logger.LogInformation("New match added for {Date}", model.Date);

                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding match");
                ModelState.AddModelError("", "An error occurred while saving the match. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            if (HttpContext.Session.GetString("LoginType") != "admin")
            {
                return RedirectToAction(nameof(Dashboard));
            }

            try
            {
                await _matchService.DeleteMatchAsync(id);
                _logger.LogInformation("Match with ID {Id} deleted", id);
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting match with ID {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the match.";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private DateTime GetNextThursday()
        {
            var today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilThursday = ((int)DayOfWeek.Thursday - (int)today.DayOfWeek + 7) % 7;
            return today.AddDays(daysUntilThursday == 0 ? 7 : daysUntilThursday);
        }
    }
}