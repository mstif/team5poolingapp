using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts;
using Services.Contracts.Helpers;
namespace Services.Abstractions
{
    public interface IGeoService
    {
        Task<List<Coordinates>> GetCoordinatesAddress(string address);
        Task<RoadTotalParams?> GetGeoData(List<InvoiceDto>? invoices);
    }
}
