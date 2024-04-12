using MediatR;
using Microsoft.Extensions.Configuration;

namespace PhotoLibrary.Ml.LabelPredictor;

public class ConfigurationService : IRequestHandler<GetConnectionStringRequest, string>
{
    public Task<string> Handle(GetConnectionStringRequest request, CancellationToken cancellationToken)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var connectionString = configuration.GetConnectionString("photo-db");
        return Task.FromResult(connectionString ?? "Empty");
    }
}
