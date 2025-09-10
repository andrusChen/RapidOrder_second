using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;

namespace RapidOrder.Infrastructure
{
    public class RapidOrderDbContext : DbContext
    {
        public RapidOrderDbContext(DbContextOptions<RapidOrderDbContext> options) : base(options)
        {
        }
        public DbSet<PlaceGroup> PlaceGroups => Set<PlaceGroup>();
        public DbSet<Place> Places => Set<Place>();
        public DbSet<CallButton> CallButtons => Set<CallButton>();
        public DbSet<Mission> Missions => Set<Mission>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Watch> Watches => Set<Watch>();
        public DbSet<EventLog> EventLogs => Set<EventLog>();
        public DbSet<ActionMap> ActionMaps => Set<ActionMap>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RapidOrderDbContext).Assembly);

            modelBuilder.Entity<Place>().HasIndex(p => p.Number).IsUnique(false);
            modelBuilder.Entity<CallButton>().HasIndex(cb => cb.DeviceCode).IsUnique();
            modelBuilder.Entity<ActionMap>().HasIndex(am => new { am.DeviceCode, am.ButtonNumber }).IsUnique();

            modelBuilder.Entity<Mission>().HasIndex(m => new { m.PlaceId, m.StartedAt });
            modelBuilder.Entity<Place>().HasData(
                new Place { Id = 1, Number = 101, Description = "Table 1", PlaceGroupId = null },
                new Place { Id = 2, Number = 102, Description = "Table 2", PlaceGroupId = null }
            );
            modelBuilder.Entity<CallButton>().HasData(
                new CallButton
                {
                    Id = 1,
                    DeviceCode = "ACEF",   // HEX code from RF
                    Label = "Table 1 Button",
                    ButtonId = "ACEF",
                    PlaceId = 1            // Must match an existing Place!
                },
                new CallButton
                {
                    Id = 2,
                    DeviceCode = "4D3F",
                    Label = "Table 2 Button",
                    ButtonId = "4D3E",
                    PlaceId = 2
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
