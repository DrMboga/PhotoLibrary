using Microsoft.AspNetCore.Authentication;

namespace PhotoLibraryBackend.Tests;

public class TestAuthHandlerOptions: AuthenticationSchemeOptions
{
    public string DefaultUserId { get; set; } = null!;
}
