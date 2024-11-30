namespace api.Models.CargoType
{
    public class CargoTypeModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public bool IsAlive { get; set; }
    }
}
