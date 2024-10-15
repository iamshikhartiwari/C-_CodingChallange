namespace LoanManagementSystem.Entity;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public int CreditScore { get; set; }
}