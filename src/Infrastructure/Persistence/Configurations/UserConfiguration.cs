using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // builder
        //     .HasMany(x => x.Events)
        //     .WithOne(x => x.Author)
        //     .HasForeignKey(x => x.AuthorId);
    }
}