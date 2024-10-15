using LoanManagementSystem.Entity;
using LoanManagementSystem.BussnissLogicLayer.Repository;

namespace LoanManagementSystem.BussnissLogicLayer.Services;

public class LoanServices : ILoanServices
{
    private ILoanServices _loanServicesImplementation;

    private readonly ILoanRepository  _loanRepository = new LoanRepository();
    public int CalculateInterest(int LoanId)
    {
        return _loanRepository.CalculateInterest( LoanId);
        // return _.CalculateInterest(LoanId);
    }

    public void loanStatus(int loanId)
    {
        _loanRepository.loanStatus(loanId);
    }

    public void applyLoan(Loan loan)
    {
        _loanRepository.applyLoan(loan);
    }

    public void loanRepayment(int loanId, decimal amount)
    {
        _loanRepository.loanRepayment(loanId, amount);
    }

    public void getAllLoan()
    {
        _loanRepository.getAllLoan();
    }

    public void getLoanById(int loanId)
    {
        _loanRepository.getLoanById(loanId);
    }
}