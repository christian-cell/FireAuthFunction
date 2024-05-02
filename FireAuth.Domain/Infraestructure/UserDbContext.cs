using FireAuth.Domain.Entities;
using FireAuth.Domain.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FireAuth.Domain.Infraestructure
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfiguration(new UserEntityConfig());
            modelBuilder.ApplyConfiguration(new UserSessionEntityConfig());
        }
        
        /*public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
        {
            public UserDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
                optionsBuilder.UseSqlServer("");

                return new UserDbContext(optionsBuilder.Options);
            }
        }*/
    }
};

