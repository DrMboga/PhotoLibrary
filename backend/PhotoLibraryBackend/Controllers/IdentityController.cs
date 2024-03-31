using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PhotoLibraryBackend;

[ApiController]
[Route("identity/[action]")]
public class IdentityController: ControllerBase
{
    private readonly IdentityDbContext _identityDbContext;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityController(IdentityDbContext identityDbContext, SignInManager<IdentityUser> signInManager)
    {
        _identityDbContext = identityDbContext;
        _signInManager = signInManager;
    }

    [HttpPost()]
    public async Task MigrateIdentityDb()
    {
        await _identityDbContext.Database.MigrateAsync();
    }

    [HttpPost()]
    public async Task<IActionResult> Logout([FromBody]object empty)
    {
        if (empty != null)
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        return Unauthorized();
    }
}
