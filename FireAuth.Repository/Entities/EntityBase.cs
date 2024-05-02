namespace FireAuth.Repository.Entities
{
    public abstract class EntityBase : IEntityBase
    {
        protected EntityBase()
        {
            Id = Guid.NewGuid();
            CreatedOn = DateTime.Now;
            CreatedByUser = "System";
        }

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

