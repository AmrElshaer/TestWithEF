using Microsoft.AspNetCore.Identity;
using TestWithEF.ValueObjects;

namespace TestWithEF.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    private ApplicationUser() { }

    public static ApplicationUser Create(Email email)
    {
        var user = new ApplicationUser
        {
            UserName = email.Value,
            Email = email.Value,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        return user;
    }
}
