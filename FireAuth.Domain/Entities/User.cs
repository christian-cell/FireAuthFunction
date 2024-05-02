using FireAuth.Repository.Entities;

namespace FireAuth.Domain.Entities
{
    public class User : EntityBase
    {
        public User()
        {
            UserSessions = new HashSet<UserSession>();
            
        }
        
        public virtual ICollection<UserSession> UserSessions { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public bool Active { get; set; }
        public UserStatus Status { get; set; }
    }
};

