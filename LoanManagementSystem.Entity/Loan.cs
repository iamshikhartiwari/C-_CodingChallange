namespace LoanManagementSystem.Entity;

public class Loan
{
    public int LoanId { get; set; }
    public Customer Customer { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int LoanTerm { get; set; }  
    public string LoanType { get; set; }
    public string LoanStatus { get; set; }
    
    public Loan(int loanId, Customer customer, decimal principalAmount, decimal interestRate, int loanTerm, string loanType, string loanStatus)
    {
        LoanId = loanId;
        Customer = customer;
        PrincipalAmount = principalAmount;
        InterestRate = interestRate;
        LoanTerm = loanTerm;
        LoanType = loanType;
    }
}