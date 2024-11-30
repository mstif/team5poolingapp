namespace Services.Contracts
{
    public class CargoTypeDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public bool IsAlive { get; set; }

    }
}
