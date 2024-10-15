namespace LoanManagementSystem.ExceptionHandling;

public class InvalidLoanException : Exception
{
    public InvalidLoanException(string message) : base(message)
    {
        
    }
}
