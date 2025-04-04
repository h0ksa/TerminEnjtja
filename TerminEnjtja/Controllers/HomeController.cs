using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            _matchService = matchService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (model.Password == AdminPassword)
            {
                // Admin login
                HttpContext.Session.SetString("LoginType", "admin");
                return RedirectToAction(nameof(Dashboard));
            }
            else if (model.Password == ViewerPassword)
            {
                // Viewer login
                HttpContext.Session.SetString("LoginType", "viewer");
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                ModelState.AddModelError("", "Incorrect password! Please try again.");
                return View("Index", model);
            }
        }

        public async Task<IActionResult> Dashboard()
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


            var match = new Match
            {
                Date = model.Date,
                TeamAScore = model.TeamAScore,
                TeamBScore = model.TeamBScore,
                Notes = model.Notes
            };

            await _matchService.AddMatchAsync(match);

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            if (HttpContext.Session.GetString("LoginType") != "admin")
            {
                return RedirectToAction(nameof(Dashboard));
            }

            await _matchService.DeleteMatchAsync(id);
            return RedirectToAction(nameof(Dashboard));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        private DateTime GetNextThursday()
        {
            var now = DateTime.Now;
            var daysUntilThursday = ((int)DayOfWeek.Thursday - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilThursday == 0) daysUntilThursday = 7;
            return now.Date.AddDays(daysUntilThursday);
        }
    }
}
