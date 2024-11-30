using GeoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Contracts.Helpers;
using Test;

namespace GeoService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoController : ControllerBase
    {
        private readonly IGeoService _service;
        public GeoController(IGeoService service)
        {
            _service = service;
        }
        [HttpPost("total-distance")]
        public async Task<RoadTotalParams> GetTotalDistance(List<Coordinates> coordinates)
        {
            var result = await _service.CalcRoadTotalParams(coordinates);
            return result;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            List<Coordinates> coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates() { Latitude = "55.739042", Longitude = "37.574795" });
            coordinates.Add(new Coordinates() { Latitude = "55.755246", Longitude = "37.617779" });
            coordinates.Add(new Coordinates() { Latitude = "55.715677", Longitude = "37.552166" });
            var result = await _service.CalcRoadTotalParams(coordinates);
            return Ok("Distance = "+ result.Distance);
        }
    }
}
