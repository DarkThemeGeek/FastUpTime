namespace FastUpTime.Models;

public struct AccountResponse(int id, string accountName)
{
    public int Id { set; get; } = id;
    public string AccountName { set; get; } = accountName;
}