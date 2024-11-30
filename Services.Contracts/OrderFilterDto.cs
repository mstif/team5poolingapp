using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public class OrderFilterDto
    {
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public string? Status {  get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public bool NotActiveFilter { get; set; } = false;
    }
}
