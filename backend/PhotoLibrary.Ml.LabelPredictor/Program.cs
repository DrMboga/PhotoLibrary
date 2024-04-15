
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PhotoLibrary.Ml.LabelPredictor;

var serviceCollection = new ServiceCollection()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
    .BuildServiceProvider();

var mediator = serviceCollection.GetRequiredService<IMediator>();

var connectionString = await mediator.Send(new GetConnectionStringRequest());

var imagePath = @"C:\Users\mikhail.shabanov\source\repos\DrMboga\photo-library-lib\To print\IMG_0335.JPG";

var label = await mediator.Send(new PredictLabelRequest(imagePath));
// mediaFileInfo.TagLabel = label.Label;

Console.WriteLine(label.Label);
