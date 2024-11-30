namespace api.Models
{
    public class EntityModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
    }

    public class EntityModelGuid
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
    }
}
