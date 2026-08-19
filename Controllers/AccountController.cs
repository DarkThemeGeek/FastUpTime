using System.Security.Cryptography;
using FastUpTime.Data;
using FastUpTime.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FastUpTime.Controllers;
[Route("/acc")]
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
    public async Task<IActionResult> Create([FromBody] UserAccount userAccount)
    {
        if (string.IsNullOrWhiteSpace(userAccount.UserName) ||
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

        await _dbContext.UserAccounts.AddAsync(userAccount);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByIdResponse), new { id = userAccount.Id });
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
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userAccount = await GetById(id);
        if (userAccount is null)
        {
            return NotFound();
        }

        _dbContext.UserAccounts.Remove(userAccount);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}