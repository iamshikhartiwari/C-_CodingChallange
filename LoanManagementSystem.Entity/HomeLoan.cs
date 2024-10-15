namespace LoanManagementSystem.Entity;

public class HomeLoan : Loan
{
    public string PropertyAddress { get; set; }
    public decimal PropertyValue { get; set; }

}