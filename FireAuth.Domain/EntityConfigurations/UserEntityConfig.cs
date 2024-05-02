using FireAuth.Domain.Entities;
using FireAuth.Repository.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FireAuth.Domain.EntityConfigurations
{
    public class UserEntityConfig : BaseEntityConfiguration<User>
    {
        protected override void ConfigureDomainEntity(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.DocumentNumber).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            
            builder.HasMany(u=> u.UserSessions) 
                .WithOne(us => us.User) 
                .HasForeignKey(u => u.UserId);
            
            builder.ToTable("Users" , "core");
        }
    }
};

