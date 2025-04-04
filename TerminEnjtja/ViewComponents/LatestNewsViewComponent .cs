using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerminEnjtja.Models;

namespace TerminEnjtja.ViewComponents
{
    public class LatestNewsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // In a real application, you would fetch this from a database
            var newsItems = new List<NewsItem>
            {
                new NewsItem
                {
                    Id = 1,
                    Title = "Player X injured ahead of derby",
                    Preview = "Key player will miss Thursday's match due to injury during training.",
                    ImageUrl = "/images/news/injury.jpg",
                    PublishedDate = DateTime.Now.AddDays(-1)
                },
                new NewsItem
                {
                    Id = 2,
                    Title = "ACI signs new goalkeeper",
                    Preview = "Team ACI has announced a new addition to strengthen their defense.",
                    ImageUrl = "/images/news/goalkeeper.jpg",
                    PublishedDate = DateTime.Now.AddDays(-3)
                },
                new NewsItem
                {
                    Id = 3,
                    Title = "BESI's winning streak continues",
                    Preview = "BESI team extends their winning streak to four matches.",
                    ImageUrl = "/images/news/celebration.jpg",
                    PublishedDate = DateTime.Now.AddDays(-5)
                },
                new NewsItem
                {
                    Id = 4,
                    Title = "Upcoming tournament announced",
                    Preview = "New tournament will feature both ACI and BESI teams next month.",
                    ImageUrl = "/images/news/tournament.jpg",
                    PublishedDate = DateTime.Now.AddDays(-7)
                }
            };

            return View(newsItems);
        }
    }

    public class FeaturedPhotosViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // In a real application, you would fetch this from a database
            var photos = new List<PhotoItem>
            {
                new PhotoItem
                {
                    Id = 1,
                    Title = "Match Highlight",
                    ImageUrl = "/images/photos/match1.jpg",
                    UploadDate = DateTime.Now.AddDays(-2)
                },
                new PhotoItem
                {
                    Id = 2,
                    Title = "Team Celebration",
                    ImageUrl = "/images/photos/celebration.jpg",
                    UploadDate = DateTime.Now.AddDays(-3)
                },
                new PhotoItem
                {
                    Id = 3,
                    Title = "Training Session",
                    ImageUrl = "/images/photos/training.jpg",
                    UploadDate = DateTime.Now.AddDays(-5)
                },
                new PhotoItem
                {
                    Id = 4,
                    Title = "Fan Support",
                    ImageUrl = "/images/photos/fans.jpg",
                    UploadDate = DateTime.Now.AddDays(-6)
                }
            };

            return View(photos);
        }
    }
}