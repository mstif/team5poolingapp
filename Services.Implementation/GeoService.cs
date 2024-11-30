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
using System.Net;
using Services.Contracts;
using System.Threading.Tasks.Dataflow;
using System.Net.Http.Json;

namespace Services.Implementation
{
    public class GeoService : IGeoService
    {

        public GeoService() {

        }
        public async Task<List<Coordinates>> GetCoordinatesAddress(string address)
        {

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
            if (tmpRes.Count() == 0)
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

        public async Task<RoadTotalParams?> GetGeoData(List<InvoiceDto> invoices)
        {

            List<Coordinates>? coordinates = invoices?.Select(i=> new Coordinates()
            {
                Latitude = i.DeliveryPoint?.Latitude ?? "",
                Longitude = i.DeliveryPoint?.Longitude ?? "",
                Address = i.DeliveryPoint?.Address ?? ""
            }).ToList();
            HttpClient httpClient = new HttpClient();
            string requestAddress = $"http://geoservice:8080/api/geo/total-distance";
            JsonContent content = JsonContent.Create(coordinates);
            // отправляем запрос
             var response = await httpClient.PostAsync(requestAddress, content);
            RoadTotalParams? data = await response.Content.ReadFromJsonAsync<RoadTotalParams>();
            return data;

        }
    }
}
