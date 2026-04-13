using EvaluationMatricesPOC.Models;
using Microsoft.EntityFrameworkCore;
namespace EvaluationMatricesPOC.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}