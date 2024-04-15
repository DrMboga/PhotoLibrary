
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotoLibrary.Ml.LabelPredictor;
using PhotoLibraryBackend.Common;
using PhotoLibraryBackend.Data;

const int BunchOfMediasSize = 100;

var connectionString = ConfigurationService.GetConnectionString();
var rootFolderToSubstitute = ConfigurationService.GetRootFolderToSubstitute();
var rootFolderToSet = ConfigurationService.GetRootFolderToSet();

var credentials = ConfigurationService.GetSambaCredentials();

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Settings:");
Console.WriteLine($"connectionString: '{connectionString}'");
Console.WriteLine($"rootFolderToSubstitute: '{rootFolderToSubstitute}'");
Console.WriteLine($"rootFolderToSet: '{rootFolderToSet}'");
Console.ResetColor();


var serviceCollection = new ServiceCollection()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
    .AddDbContextFactory<PhotoLibraryBackendDbContext>(
            options =>
                options.UseNpgsql(connectionString))
    .AddSingleton(credentials)
    .BuildServiceProvider();

var mediator = serviceCollection.GetRequiredService<IMediator>();
int stopProcessing = 0;
var exitEvent = new ManualResetEvent(false);

// Handle Ctrl+C exit gracefully
Console.CancelKeyPress += (sender, e) => {
    Interlocked.Exchange(ref stopProcessing, 1);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("-== Cancellation requested ==-");
    Console.ResetColor();
    exitEvent.WaitOne();
    Console.WriteLine("-== Exit ==-");
};


while (true) {
    var bunchOfMedias = await mediator.Send(new GetBunchOfMediasWithEmptyLabelRequest(BunchOfMediasSize));
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{bunchOfMedias.Length} medias read from DB");
    Console.ResetColor();

    foreach (var mediaInfo in bunchOfMedias)
    {
        var relativePath = Path.GetRelativePath(rootFolderToSubstitute, mediaInfo.FullFileName);
        try
        {
            var label = await mediator.Send(new PredictLabelRequest(Path.Combine(rootFolderToSet, relativePath)));
            await mediator.Publish(new SetMediaLabelNotification(mediaInfo.MediaId, label.Label));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(label.Label);
            Console.ResetColor();
            Console.WriteLine($"\t - [{mediaInfo.MediaId}] '{Path.Combine(rootFolderToSet, relativePath)}'");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR '{Path.Combine(rootFolderToSet, relativePath)}'");
            Console.ResetColor();
            Console.WriteLine(ex.ToString());
        }

        if (stopProcessing > 0) {
            break;
        }
    }

    var labelsInfo = await mediator.Send(new GetLabeledMediasInfoRequest());
    var percentage = Convert.ToInt32(100 * Convert.ToDecimal(labelsInfo.labeledMediaCount) / Convert.ToDecimal(labelsInfo.totalMediaCount));

    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"{labelsInfo.labeledMediaCount:N0} from {labelsInfo.totalMediaCount} labels marked ({percentage}%)");
    Console.ResetColor();

    if (stopProcessing > 0) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Exit requested");
        Console.ResetColor();
        exitEvent.Set();
        break;
    }
}

