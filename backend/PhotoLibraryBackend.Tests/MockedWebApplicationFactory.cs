using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace PhotoLibraryBackend.Tests;

public class MockedWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public Mock<IMediator> MockMediator { get; } = new Mock<IMediator>();
    public Mock<IImporterService> MockImporter { get; } = new Mock<IImporterService>();
    public FakeAuthorizationHandler FakeAuthHandler { get; } = new FakeAuthorizationHandler();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
        builder.ConfigureServices(services =>
        {
            // Mock auth handler (https://mazeez.dev/posts/auth-in-integration-tests)
            services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = "mike@photo-library.com");
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });

            // Mock specific ConfirmedEmailPolicy
            var authHandler = services.Single(d => d.ImplementationType == typeof(ShouldHaveConfirmedEmailRequirementAuthorizationHandler));
            if (authHandler != null)
            {
                services.Remove(authHandler);
                services.AddSingleton<IAuthorizationHandler>(FakeAuthHandler);
                services.AddAuthorization(options => {
                    options.AddPolicy("EmailShouldBeConfirmed", policy => {
                        policy.Requirements.Add(new ShouldHaveConfirmedEmailRequirement());
                    });
                });
            }

            // Mock mediatR
            var mediatorService = services.SingleOrDefault(d => d.ServiceType == typeof(IMediator));
            if (mediatorService != null)
            {
                services.Remove(mediatorService);
                services.AddSingleton(MockMediator.Object);
            }

            // Mock importer service
            var importerService = services.SingleOrDefault(d => d.ServiceType == typeof(IImporterService));
            if (importerService != null)
            {
                services.Remove(importerService);
                services.AddSingleton(MockImporter.Object);
            }
        });

        builder.UseEnvironment("Development");
    }
}
