using Microsoft.EntityFrameworkCore;

namespace wedding_planner.Models
{
    public class WeddingPlannerContext : DbContext
    {
        public WeddingPlannerContext(DbContextOptions options) : base(options) { }
        // tables in db
        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }

        // LoginUser doesn't neet to get mapped to DB
    }
}