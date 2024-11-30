using ApplicationCore.Entities.Base;

namespace ApplicationCore.Base
{
    public  abstract class EntityBook : BaseEntity
    {
        public virtual string Name { get; set; } = null!;
    }
}
