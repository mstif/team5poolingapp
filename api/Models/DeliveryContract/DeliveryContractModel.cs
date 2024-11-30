﻿using Services.Contracts;
using System.ComponentModel.DataAnnotations;

namespace api.Models.DeliveryContract
{
    public class DeliveryContractModel
    {
        public long Id { get; set; } = 0;
        public virtual string? Comment { get; set; }
        public bool IsAlive { get; set; } = true;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; } = DateTime.MinValue;
        public string Number { get; set; } = string.Empty;
        public string? Title { get; set; } = "Новый документ";
        public long? LogisticCompanyId { get; set; }
        public virtual ContragentDto? LogisticCompany { get; set; }
        public ContragentDto? Seller;
        public DateTime? DateDelivery { get; set; }
        public decimal TotalCostDelivery { get; set; }
        public string Status { get; set; }
        public OrderDto? Order { get; set; }
        public long? OrderId { get; set; }
    }
}