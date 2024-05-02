using FireAuth.Domain.Entities;
using FireAuth.Repository.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FireAuth.Domain.EntityConfigurations
{
    public class UserSessionEntityConfig : BaseEntityConfiguration<UserSession>
    {
        protected override void ConfigureDomainEntity(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(us => us.Id);

            builder.Property(us => us.RefreshToken).IsRequired(); 
            builder.Property(us => us.RefreshTokenExpirationDate).IsRequired(); 

            builder.HasOne(us => us.User) // Configures relationship with Customer entity
                .WithMany(u => u.UserSessions)
                .HasForeignKey(us => us.UserId);
            
            builder.ToTable("UserSessions", "core");
        }
    }
};

