using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RapidOrder.Infrastructure
{
    public class RapidOrderDbContextFactory : IDesignTimeDbContextFactory<RapidOrderDbContext>
    {
        public RapidOrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RapidOrderDbContext>();

            // ⚠️ Use the same connection string you use in Program.cs
            optionsBuilder.UseSqlite("Data Source=rapidorder.db");

            return new RapidOrderDbContext(optionsBuilder.Options);
        }
    }
}
