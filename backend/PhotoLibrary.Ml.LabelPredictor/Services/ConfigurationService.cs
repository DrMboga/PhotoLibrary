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

    public static string GetRootFolderToSubstitute()
    {
        // RootFolderToSubstitute
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var rootFolderToSubstitute = configuration.GetSection("LabelPredictor:RootFolderToSubstitute").Value;
        return rootFolderToSubstitute ?? "Empty";
    }

    public static string GetRootFolderToSet()
    {
        // RootFolderToSet
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var rootFolderToSet = configuration.GetSection("LabelPredictor:RootFolderToSet").Value;
        return rootFolderToSet ?? "Empty";

    }

    public static SambaCredentials GetSambaCredentials()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var login = configuration.GetSection("LabelPredictor:SambaLogin").Value;
        var password = configuration.GetSection("LabelPredictor:SambaPassword").Value;
        return new SambaCredentials(login ?? string.Empty, password ?? string.Empty);
    }
}
