using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace PhotoLibraryBackend.Tests;

public class MockedWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(m => m.Send(It.IsAny<GetLibraryInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediatRMessagesMock.LibraryInfoRequestMock());

        // https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        builder.ConfigureServices(services =>
        {
            var mediatorService = services.SingleOrDefault(d => d.ServiceType == typeof(IMediator));
            if (mediatorService != null)
            {
                services.Remove(mediatorService);
                services.AddSingleton(mockMediator.Object);
            }
        });

        builder.UseEnvironment("Development");
    }
}
