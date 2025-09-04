using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RapidOrder.Core.Entities;

namespace RapidOrder.Infrastructure.Config
{
    public class PlaceConfig : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> b)
        {
            b.Property(p => p.Description).HasMaxLength(128);
        }
    }
}
