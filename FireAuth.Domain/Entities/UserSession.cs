using FireAuth.Repository.Entities;

namespace FireAuth.Domain.Entities
{
    public class UserSession : EntityBase
    {
        public Guid UserId { get; set; } 
        public User User { get; set; } 
        public string RefreshToken { get; set; } 
        public DateTime RefreshTokenExpirationDate { get; set; } 
        public bool Used { get; set; }
    }
};

