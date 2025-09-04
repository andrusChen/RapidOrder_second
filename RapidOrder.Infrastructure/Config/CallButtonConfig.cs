using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RapidOrder.Core.Entities;

namespace RapidOrder.Infrastructure.Config
{
    public class CallButtonConfig : IEntityTypeConfiguration<CallButton>
    {
        public void Configure(EntityTypeBuilder<CallButton> b)
        {
            b.Property(x => x.DeviceCode).HasMaxLength(64);
            b.Property(x => x.Label).HasMaxLength(64);
        }
    }
}
