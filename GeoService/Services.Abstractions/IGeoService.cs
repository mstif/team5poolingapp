using GeoService.Models;
using Services.Contracts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace Services.Abstractions
{
    public interface IGeoService
    {
        Task<List<Coordinates>> GetCoordinatesAddress(string address);
        Task<RoadTotalParams> CalcRoadTotalParams(List<Coordinates> coordinates);
    }
}
