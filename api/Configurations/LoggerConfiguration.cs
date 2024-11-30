using Serilog;
namespace api.Configurations
{
    public static class LoggerConfiguration
    {
        public static IServiceCollection AddLogger(this IServiceCollection serviceCollection, ConfigurationManager configuration)
        {
            var options = configuration.GetSection(SerilogOptions.Position).Get<SerilogOptions>();

            Log.Logger = new Serilog.LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Seq(options.UrlSeq)
                .CreateLogger();

            serviceCollection.AddSingleton(Log.Logger);

            return serviceCollection;
        }
    }
}
