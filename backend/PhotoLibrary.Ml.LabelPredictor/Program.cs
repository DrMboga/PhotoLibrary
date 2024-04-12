
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PhotoLibrary.Ml.LabelPredictor;

var serviceCollection = new ServiceCollection()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
    .BuildServiceProvider();

var mediator = serviceCollection.GetRequiredService<IMediator>();

var connectionString = await mediator.Send(new GetConnectionStringRequest());

Console.WriteLine(connectionString);
