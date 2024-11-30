namespace ApplicationCore.Entities.Base
{
    public class BaseEntity
    {
        public long Id { get; set; } = 0;
        public virtual string? Comment { get; set; }
        public bool IsAlive { get; set; } = true;
    }
}
