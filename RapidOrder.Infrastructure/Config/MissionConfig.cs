using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RapidOrder.Core.Entities;

namespace RapidOrder.Infrastructure.Config
{
    public class MissionConfig : IEntityTypeConfiguration<Mission>
    {
        public void Configure(EntityTypeBuilder<Mission> b)
        {
            b.Property(m => m.SourceDecoded).HasMaxLength(64);
        }
    }
}
