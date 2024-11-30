using Services.Abstractions;
using Services.Contracts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using System.Text.Json;
using Services.Contracts;
using Test;

namespace Services.Implementation
{
    public class GeoServiceData:IGeoService
    {
  
        private readonly IHttpClientFactory _clientFactory;

        public GeoServiceData( IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<List<Coordinates>> GetCoordinatesAddress(string address) {

            HttpClient httpClient = new HttpClient();

            string requestAddress = $"https://geocode-maps.yandex.ru/1.x/?geocode={address}&format=json&results=10&apikey=9cd192cd-2fd4-4f16-95b2-c3b67d917002";
            // string requestAddress = "https://www.google.com";
            // определяем данные запроса
            //using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestAddress);

            //// получаем ответ
            //using HttpResponseMessage response = await httpClient.SendAsync(request);

            //// просматриваем данные ответа
            //// статус
            //Console.WriteLine($"Status: {response.StatusCode}\n");
            ////заголовки
            //Console.WriteLine("Headers");
            //foreach (var header in response.Headers)
            //{
            //    Console.Write($"{header.Key}:");
            //    foreach (var headerValue in header.Value)
            //    {
            //        Console.WriteLine(headerValue);
            //    }
            //}
            //// содержимое ответа
            //Console.WriteLine("\nContent");
            //string content = await response.Content.ReadAsStringAsync();
            string content = await httpClient.GetStringAsync(requestAddress);
            Console.WriteLine(content);
           var documentJson = JsonDocument.Parse(content).RootElement;
            var tmpRes = documentJson.
                 GetProperty("response").
                 GetProperty("GeoObjectCollection").
                 GetProperty("featureMember").EnumerateArray();
            if (tmpRes.Count()==0)
            {
                Console.WriteLine("Не удалось распознать адрес");
                return null;
            }
            var result = new List<Coordinates>();
            foreach (var res in tmpRes)
            {
                var coord = res.GetProperty("GeoObject")
                    .GetProperty("Point")
                    .GetProperty("pos").ToString().Split(" ");
                var adr = res.GetProperty("GeoObject")
                    .GetProperty("metaDataProperty")
                    .GetProperty("GeocoderMetaData")
                    .GetProperty("text").ToString();

                result.Add(new Coordinates() { Address = adr, Latitude = coord[0], Longitude = coord[1] });
            }
            
            return result;
        }

        public async Task<RoadTotalParams> CalcRoadTotalParams(List<Coordinates> coordinates)
        {
            var roadTotalParams = new RoadTotalParams();
            var coordinatesStrTasks = coordinates
                .Select(
                    x => Task<string>.Run(
                        async  () =>
                        {
                            var result = string.Empty;
                            
                            if ((string.IsNullOrEmpty(x.Longitude) || string.IsNullOrEmpty(x.Latitude))
                                && !string.IsNullOrEmpty(x.Address))
                            {
                                using (var yandexGeoClient = _clientFactory.CreateClient("YandexGeo"))
                                {
                                    var response =  await yandexGeoClient.GetAsync($"1.x/?apikey=f448ec12-e79c-4c86-94e8-afcdc42e44e3&geocode={x.Address}");
                                    response.EnsureSuccessStatusCode();
                                    var content =  await response.Content.ReadAsStringAsync();
                                    var documentJson = JsonDocument.Parse(content).RootElement;
                                    var temp = documentJson
                                        .GetProperty("response")
                                        .GetProperty("GeoObjectCollection")
                                        .GetProperty("featureMember")
                                        .EnumerateArray().GetEnumerator();
                                    if (temp.MoveNext())
                                    {
                                        var coodinatesPair = temp.Current
                                            .GetProperty("GeoObject")
                                            .GetProperty("Point")
                                            .GetProperty("pos")
                                            .GetString().Split(" ");
                                        result = string.Join(',',coodinatesPair);
                                    }
                                    else
                                    {
                                        throw new Exception("Яндекс гео е вернул координаты");
                                    }
                                    
                                }
                            }
                            else
                            {
                                result = x.Longitude + "," + x.Latitude;
                            }

                            return result;
                        })
                    );
            var coordinatesStr = string.Join(';', await Task.WhenAll(coordinatesStrTasks));
            using (var osrmDrivingClient = _clientFactory.CreateClient("OsrmDriving"))
            {

                var response = await osrmDrivingClient.GetAsync(coordinatesStr);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var documentJson = JsonDocument.Parse(content).RootElement;
                var routes = documentJson.GetProperty("routes").EnumerateArray().GetEnumerator();
                if (routes.MoveNext())
                {

                    routes.Current.GetProperty("distance").TryGetDecimal(out var distance);
                    roadTotalParams.Distance = distance;
                    routes.Current.GetProperty("duration").TryGetDecimal(out var duration);
                    roadTotalParams.Duration = duration;
                }
            }

            return roadTotalParams;
        }
        
        //public async Task<>
    }
}
