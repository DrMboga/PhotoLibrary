using System.Globalization;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoLibraryBackend;
using PhotoLibraryBackend.Data;
using Serilog;


// For deploy on Raspberry PI home server
const string HostServer = "192.168.0.65:8850";

const string AllowCors = "AllowEverything";

const string ConfirmedEmailPolicyName = "EmailShouldBeConfirmed";

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse(HostServer));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowCors,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:3000/", "http://192.168.0.65:8860", "http://192.168.0.65:8860/")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                ;
        });
});

// Configure Authentication
builder.Services.AddDbContext<IdentityDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("photo-identity-db") ?? "undefined"));
builder.Services.AddScoped<IAuthorizationHandler, ShouldHaveConfirmedEmailRequirementAuthorizationHandler>();
builder.Services.AddAuthorization(options => {
    options.AddPolicy(ConfirmedEmailPolicyName, policy => {
        policy.Requirements.Add(new ShouldHaveConfirmedEmailRequirement());
    });
});
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<IdentityDbContext>();

// Configure Serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
);

// GeolocationService
builder.Services.AddHttpClient("PositionStackApi", c =>
{
    c.BaseAddress = new Uri("http://api.positionstack.com/");
    c.DefaultRequestHeaders.Add("Accept", "*/*");
    c.DefaultRequestHeaders.Add("User-Agent", "Mike's photo library");
});

// Setup PhotoLibrary services
var settings = builder.Configuration
                .GetSection("PhotoLibrary")
                .Get<PhotoLibrarySettings>();
builder.Services.AddSingleton<PhotoLibrarySettings>(settings!);
builder.Services.AddScoped<IImporterService, ImporterService>();

builder.Services.AddTransient<ImportMediaBackgroundOperationType>();
builder.Services.AddTransient<FixVideoDatesBackgroundOperationType>();
builder.Services.AddTransient<GeocodingCollectBackgroundOperationType>();
builder.Services.AddTransient<ConvertQuickTimeVideosOperationType>();

builder.Services.AddSingleton<WorkerDispatcher>();
builder.Services.AddHostedService<WorkerService>();

builder.Services.RegisterPhotoLibraryDbContext(builder.Configuration.GetConnectionString("photo-db") ?? "undefined");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(AllowCors);
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

#region SingnalR hubs
app.MapHub<MediaHub>("/Media")
    .RequireAuthorization(ConfirmedEmailPolicyName)
    ;

app.MapHub<ImporterLoggerHub>("/ImporterLogger")
    .RequireAuthorization(ConfirmedEmailPolicyName)
    ;
app.MapHub<GeocodingLoggerHub>("/GeocodingLogger")
    .RequireAuthorization(ConfirmedEmailPolicyName)
    ;
#endregion

app.MapGroup("identity").MapIdentityApi<IdentityUser>();

// /swagger/index.html
#region photo library backend API
// Root endpoint returns text info about backend version and DB info
app.MapGet("/", async (IMediator mediator) => 
{
    var version = Assembly.GetEntryAssembly()?.GetName().Version;
    string secondLine = string.Empty;
    try {
        var libraryInfo = await mediator.Send(new GetLibraryInfoRequest());
        var ruCulture = CultureInfo.GetCultureInfo("ru-RU", false);
        string totalMedias = (libraryInfo?.MediaFilesCount ?? 0).ToString("N0", ruCulture);
        secondLine = $"There are {totalMedias} media files in the library";
        if (libraryInfo?.DateOfEarliestPhoto != null && libraryInfo?.DateOfNewestPhoto != null)
        {
            string firstDate = libraryInfo.DateOfEarliestPhoto.Value.ToString("g", ruCulture);
            string lastDate = libraryInfo.DateOfNewestPhoto.Value.ToString("g", ruCulture);
            secondLine = $"{secondLine}{Environment.NewLine}Date of last photo: {lastDate}; date of first photo: {firstDate}";
        }
    }
    catch(Exception e)
    {
        secondLine = $"There is a problem to access library: '{e.Message}'";
    }
    return $"Photo library backend version {version}{Environment.NewLine}{secondLine}";
});
#endregion

app.MapControllers();

app.Run();

// This row is here to make this class visible to integration tests
public partial class Program { }
