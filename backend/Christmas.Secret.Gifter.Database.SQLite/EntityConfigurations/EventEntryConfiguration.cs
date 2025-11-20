using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.EntityConfigurations {
    public class EventEntryConfiguration : IEntityTypeConfiguration<EventEntry>
    {
        public void Configure(EntityTypeBuilder<EventEntry> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
