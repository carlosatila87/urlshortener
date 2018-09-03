using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Models
{
    public class UrlContext : DbContext
    {
        public UrlContext(DbContextOptions<UrlContext> options)
            : base(options)
        { }

        public DbSet<Url> Urls { get; set; }        
    }

    public class Url
    {
        [Key]
        public string Token { get; set; }
        public string LongUrl { get; set; }        
    }
}


