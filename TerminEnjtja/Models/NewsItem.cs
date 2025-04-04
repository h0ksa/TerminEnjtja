using System;

namespace TerminEnjtja.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Preview { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
    }

    public class PhotoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }
    }

    public class PostItem
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatarUrl { get; set; }
        public DateTime PostDate { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int SharesCount { get; set; }
    }
}