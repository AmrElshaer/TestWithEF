using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestWithEF.Identity;
using TestWithEF.Models;
using TestWithEF.ValueObjects;

namespace TestWithEF.Controllers;

public class UserController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly JwtOptions _jwtOptions;

    public UserController
    (
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model is null)
        {
            return BadRequest("Invalid client request");
        }

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user is null)
        {
            return NotFound(model.Username);
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!isValidPassword)
        {
            return Unauthorized();
        }

        var token = await GenerateToken(user);

        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo));
    }

    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GetToken(authClaims);

        return token;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);

        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "User already exists!"
            });

        var emailResult = Email.CreateEmail(model.Email);

        if (emailResult.Failure)
        {
            return BadRequest(emailResult.Error);
        }

        var user = ApplicationUser.Create(emailResult.Value);

        if (user.Failure)
        {
            return BadRequest(user.Error);
        }

        var result = await _userManager.CreateAsync(user.Value, model.Password);

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "User creation failed! Please check user details and try again."
            });

        return Ok(new Response
        {
            Status = "Success",
            Message = "User created successfully!"
        });
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);

        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "User already exists!"
            });

        var emailResult = Email.CreateEmail(model.Email);

        if (emailResult.Failure)
        {
            return BadRequest(emailResult.Error);
        }

        var user = ApplicationUser.Create(emailResult.Value);

        if (emailResult.Failure)
        {
            return BadRequest(emailResult.Error);
        }

        var result = await _userManager.CreateAsync(user.Value, model.Password);

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "User creation failed! Please check user details and try again."
            });

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new ApplicationRole(UserRoles.Admin));

        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new ApplicationRole(UserRoles.User));

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user.Value, UserRoles.Admin);
        }

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user.Value, UserRoles.User);
        }

        return Ok(new Response
        {
            Status = "Success",
            Message = "User created successfully!"
        });
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
