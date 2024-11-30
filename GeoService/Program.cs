using Polly;
using Polly.Extensions.Http;
using Services.Abstractions;
using Services.Implementation;
namespace GeoService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IGeoService, GeoServiceData>();

            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Retrying due to: {outcome.Exception?.Message}. Wait time: {timespan}. Attempt: {retryAttempt}.");
                });
            builder.Services.AddHttpClient("YandexGeo", client =>
            {
                client.BaseAddress = new Uri($"https://geocode-maps.yandex.ru/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
                .AddPolicyHandler(retryPolicy);

            builder.Services.AddHttpClient("OsrmDriving", client =>
            {
                client.BaseAddress = new Uri($"http://osrm-server:5000/route/v1/driving/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
                .AddPolicyHandler(retryPolicy);

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
