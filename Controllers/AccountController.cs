using System.Security.Claims;
using System.Security.Cryptography;
using FastUpTime.Data;
using FastUpTime.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FastUpTime.Controllers;
[Route("/auth")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserDbContext _dbContext;
    public AccountController(UserDbContext dbContext) => _dbContext = dbContext;

    [HttpGet]
    public async Task<List<UserAccount>> GetAll()
    {
        return await _dbContext.UserAccounts.ToListAsync();
    }


    public async Task<AccountResponse?> GetByIdResponse(int id)
    {
        var account = await _dbContext.UserAccounts.FirstOrDefaultAsync((account) => account.Id == id);
        if (account != null)
        {
            return new AccountResponse(account.Id, account.UserName);
        }
        else
        {
            return null;
        }
    }
    public async Task<UserAccount?> GetById(int id)
    {
       return await _dbContext.UserAccounts.FirstOrDefaultAsync((account) => account.Id == id);
        
    }


    [HttpPost]
    [Route("/register")]
    public async Task<IActionResult> Create([FromBody] UserAccount userAccount)
    {
        
        if (string.IsNullOrWhiteSpace(userAccount.UserName) ||
            string.IsNullOrWhiteSpace(userAccount.Password))
        {
            return BadRequest("Invalid Request");
        }

        if (await _dbContext.UserAccounts.AnyAsync((acc) => acc.UserName == userAccount.UserName))
            return Conflict("Username taken");
        
        
         
        var hasher = new PasswordHasher<UserAccount>();
        
        string hashPassword = hasher.HashPassword(
            userAccount,
            userAccount.Password
        );

        userAccount.Password = hashPassword;

        await _dbContext.UserAccounts.AddAsync(userAccount);
        await _dbContext.SaveChangesAsync();
        
        return Ok("Account created");
    }
 

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserAccount userAccount)
    {
        if (userAccount.Id == 0 || string.IsNullOrWhiteSpace(userAccount.UserName) ||
            string.IsNullOrWhiteSpace(userAccount.Password))
        {
            return BadRequest("Invalid Request");
        }

        var hasher = new PasswordHasher<UserAccount>();

        string hashPassword = hasher.HashPassword(
            userAccount,
            userAccount.Password
        );

        userAccount.Password = hashPassword;

        _dbContext.UserAccounts.Update(userAccount);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    // [HttpDelete("{id:int}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     var userAccount = await GetById(id);
    //     if (userAccount is null)
    //     {
    //         return NotFound();
    //     }
    //
    //     _dbContext.UserAccounts.Remove(userAccount);
    //     await _dbContext.SaveChangesAsync();
    //     return Ok();
    // }
    [HttpGet("/denied")]
    public IActionResult Deny()
    {
        return Forbid();
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            name = User.FindFirstValue(ClaimTypes.Name),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
    
    [HttpPost]
    [Route("/login")]
    public async Task<IActionResult> Login([FromBody] UserAccount userAccount)
    {
        if (userAccount.Id == 0 || string.IsNullOrWhiteSpace(userAccount.UserName) ||
            string.IsNullOrWhiteSpace(userAccount.Password))
        {
            return BadRequest("Invalid Request");
        }
        var hasher = new PasswordHasher<UserAccount>();

        string hashPassword = hasher.HashPassword(
            userAccount,
            userAccount.Password
        );
        
        var result=await _dbContext.UserAccounts.AnyAsync((acc) =>
            acc.UserName == userAccount.UserName && acc.Password == userAccount.Password);
        if (!result)
        {
            return Unauthorized("Wrong username and/or password");
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
            new Claim(ClaimTypes.Name, userAccount.UserName),
            new Claim(ClaimTypes.Role, userAccount.Role.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);
        
        await HttpContext.SignInAsync(   CookieAuthenticationDefaults.AuthenticationScheme,
            principal);
        
        
        return Ok("Logged in");
    }
}