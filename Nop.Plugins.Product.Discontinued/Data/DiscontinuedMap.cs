using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Product.Discontinued.Domain;

namespace Nop.Plugin.Product.Discontinued.Data
{
    public class DiscontinuedMap : NopEntityTypeConfiguration<DiscontinuedStatus>
    {
        public override void Configure(EntityTypeBuilder<DiscontinuedStatus> builder)
        {
            builder.ToTable(nameof(DiscontinuedStatus));
            builder.HasKey(i => i.Id);
            builder.Property(s => s.DiscontinuedState);
            builder.Property(i => i.ModelRef);
        }
    }
}
