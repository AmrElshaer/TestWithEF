using Microsoft.AspNetCore.Identity;

namespace TestWithEF.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    private ApplicationRole()
    : base()
    {
        
    }
    public ApplicationRole(string roleName) : base(roleName)
    {
        
    }
}
