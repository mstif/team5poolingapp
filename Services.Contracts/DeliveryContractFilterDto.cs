﻿namespace Services.Contracts
{
    public class DeliveryContractFilterDto
    {
        public string? Title { get; set; }
        public string? Status { get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public bool NotActive { get; set; } = false;
    }
}
