using System.Linq.Expressions;

namespace FireAuth.Repository.Entities
{
    public interface IEntityBase
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string CreatedByUser { get; set; }
        public string? ModifiedByUser { get; set; }
        public string? DeletedByUser { get; set; }
        public bool Deleted { get; set; }
    }
};

