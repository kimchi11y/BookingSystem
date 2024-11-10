using BookingSystem.API.DataTransferObjects;
using BookingSystem.Entities.DbContext;
using BookingSystem.Entities.Model;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController:ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly BookingSystemDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AuthController(ILogger<AuthController> logger, BookingSystemDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
        if (!result.Succeeded)
        {
            return BadRequest("Password is incorrect");
        }
        
        var token = await _userManager.GenerateUserTokenAsync(user, "BookingSystem", "access_token");
        return Ok(new {token, UserDto = user.Adapt<UserDto>()});
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new ApplicationUser { Email = registerDto.Email, UserName = registerDto.UserName,Name = registerDto.Name,Phone = registerDto.Phone };
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(user.Adapt<UserDto>());
    }
}