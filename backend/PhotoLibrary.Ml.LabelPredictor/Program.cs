
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotoLibrary.Ml.LabelPredictor;
using PhotoLibraryBackend.Common;
using PhotoLibraryBackend.Data;

const int BunchOfMediasSize = 10;

var connectionString = ConfigurationService.GetConnectionString();

var serviceCollection = new ServiceCollection()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
    .AddDbContextFactory<PhotoLibraryBackendDbContext>(
            options =>
                options.UseNpgsql(connectionString))
    .BuildServiceProvider();

var mediator = serviceCollection.GetRequiredService<IMediator>();

var bunchOfMedias = await mediator.Send(new GetBunchOfMediasWithEmptyLabelRequest(BunchOfMediasSize));

foreach (var mediaInfo in bunchOfMedias)
{
    var label = await mediator.Send(new PredictLabelRequest(mediaInfo.FullFileName));
    Console.WriteLine($" - '{label.Label}': {mediaInfo.MediaId}-'{mediaInfo.FullFileName}'");
    // mediaFileInfo.TagLabel = label.Label;
}
