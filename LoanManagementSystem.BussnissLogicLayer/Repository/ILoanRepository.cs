using LoanManagementSystem.Entity;

namespace LoanManagementSystem.BussnissLogicLayer.Repository;

public interface ILoanRepository
{
    
    
    public int CalculateInterest(int LoanId);
    public void loanStatus(int loanId);
    public void applyLoan(Loan loan);
    public void loanRepayment(int loanId, decimal amount);
    public void getAllLoan();
    public void getLoanById(int loanId);
    
    
}