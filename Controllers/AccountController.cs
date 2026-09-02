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
    //[Authorize("Admin")]
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


    [HttpPost("/auth/register")]
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
    [Authorize]
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
    //     {-
    //         return NotFound();
    //     }
    //
    //     _dbContext.UserAccounts.Remove(userAccount);
    //     await _dbContext.SaveChangesAsync();
    //     return Ok();
    // }
    [HttpGet("/auth/denied")]
    public IActionResult Deny()
    {
        return Forbid();
    }
    [Authorize]
    [HttpGet("/auth/me")]
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
    [AllowAnonymous]
    [Route("/auth/login")]
    public async Task<IActionResult> Login([FromBody] UserAccount login)
    {
        if (string.IsNullOrWhiteSpace(login.UserName) ||
            string.IsNullOrWhiteSpace(login.Password))
        {
            return BadRequest("Invalid request");
        }

        var user = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.UserName == login.UserName);

        if (user == null)
        {
            return Unauthorized("Wrong username and/or password");
        }

        var hasher = new PasswordHasher<UserAccount>();

        var result = hasher.VerifyHashedPassword(
            user,
            user.Password,
            login.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Wrong username and/or password");
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.UserName
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()
            )
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return Ok();
    }
}