using Microsoft.EntityFrameworkCore;
using pro_settlements.Models;

namespace pro_settlements.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Settlement> Settlements { get; set; }
    }
}
