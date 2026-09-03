namespace FastUpTime.Models;

public struct AccountResponse(long id, string accountName)
{
    public long Id { set; get; } = id;
    public string AccountName { set; get; } = accountName;
}