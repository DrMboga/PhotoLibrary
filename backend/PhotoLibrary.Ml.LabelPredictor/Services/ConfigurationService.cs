using Microsoft.Extensions.Configuration;

namespace PhotoLibrary.Ml.LabelPredictor;

public static class ConfigurationService
{
    public static string GetConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var connectionString = configuration.GetConnectionString("photo-db");
        return connectionString ?? "Empty";
    }
}
