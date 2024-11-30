using ApplicationCore.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Base
{
    public abstract class EntityDocument : BaseEntity
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public  DateTime? Date { get; set; } = DateTime.MinValue;
        public string Number { get; set; } = string.Empty;
        public string? Title { get; set; } = "Новый документ";
    }
}
