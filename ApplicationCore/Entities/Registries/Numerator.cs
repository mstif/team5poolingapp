
using ApplicationCore.Books;
using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Registries
{
    public class Numerator
    {
        public int Id { get; set; }
        public ObjectTypes ObjectType{ get; set; }
        public bool IsYearPeriod {  get; set; }
        public DateTime DateIssue { get; set; }
        public string? CurrentStringNumber { get; set; }
        public long? CurrentLongNumber { get; set; }
        public long SellerID {  get; set; }
        [ForeignKey("SellerID")]
        public virtual Contragent? Seller { get; set; }



    }
}
