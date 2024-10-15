namespace LoanManagementSystem.Entity;

public class CarLoan : Loan
{
    public string CarModel { get; set; }
    public decimal CarValue { get; set; }
}