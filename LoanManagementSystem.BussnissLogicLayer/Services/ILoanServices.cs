using LoanManagementSystem.Entity;

namespace LoanManagementSystem.BussnissLogicLayer.Services;

public interface ILoanServices
{
    public int CalculateInterest(int LoanId);
    public void loanStatus(int loanId);
    public void applyLoan(Loan loan);
    public void loanRepayment(int loanId, int amount);
    public void getAllLoan();
    public void getLoanById(int loanId);
}