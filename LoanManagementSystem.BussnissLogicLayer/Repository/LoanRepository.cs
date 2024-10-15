using LoanManagementSystem.Utils;
using System.Data.SqlClient;
using LoanManagementSystem.Entity;
using LoanManagementSystem.ExceptionHandling;
namespace LoanManagementSystem.BussnissLogicLayer.Repository;



public class LoanRepository : ILoanRepository
{
    private ILoanRepository _loanRepositoryImplementation;
    
    public int CalculateInterest(int LoanId)
    {
        using(var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            int result =0;
        

            string query = "select PrincipalAmount,InterestRate,LoanTenure from Loan";

            SqlCommand cmd = new SqlCommand(query, conn);


            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            int pa = 0;
            int ir = 0;
            int lt = 0;

            while (sqlDataReader.Read())
            {
                pa = Convert.ToInt32(sqlDataReader[0]);
                ir = Convert.ToInt32(sqlDataReader[1]);
                lt = Convert.ToInt32(sqlDataReader[2]);
                result = (int)(pa * ir * lt) / 12;
            }

            return result;

        }
    }
    
    public void loanStatus(int loanId)
    {
        
        string query = "SELECT LoanId, CustomerId, LoanStatus FROM Loans WHERE LoanId = @LoanId";

        using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LoanId", loanId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int customerId = reader.GetInt32(1);
                    string currentStatus = reader.GetString(2);

                    // Assuming we have a method to get the customer's credit score
                    int creditScore = GetCreditScore(customerId); 
                        
                    if (creditScore > 650)
                    {
                        // Update loan status to approved
                        UpdateLoanStatus(loanId, "Approved");
                        Console.WriteLine("Loan approved.");
                    }
                    else
                    {
                        // Update loan status to rejected
                        UpdateLoanStatus(loanId, "Rejected");
                        Console.WriteLine("Loan rejected due to low credit score.");
                    }
                }
                else
                {
                    throw new InvalidLoanException($"Loan with ID {loanId} not found.");
                }
            }
        }
    }



    public void applyLoan(Loan loan)
    {
        Console.WriteLine("Are you sure you want to apply for this loan? (Yes/No)");
        string confirmation = Console.ReadLine();

        if (confirmation?.Trim().ToLower() == "yes")
        {
            // Proceed to store loan details in the database
            string query = "INSERT INTO Loans (CustomerId, PrincipalAmount, InterestRate, LoanTerm, LoanType, LoanStatus) " +
                           "VALUES (@CustomerId, @PrincipalAmount, @InterestRate, @LoanTerm, @LoanType, @LoanStatus)";

            using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", loan.Customer.CustomerId); // Assuming customer has been created
                    cmd.Parameters.AddWithValue("@PrincipalAmount", loan.PrincipalAmount);
                    cmd.Parameters.AddWithValue("@InterestRate", loan.InterestRate);
                    cmd.Parameters.AddWithValue("@LoanTerm", loan.LoanTerm);
                    cmd.Parameters.AddWithValue("@LoanType", loan.LoanType);
                    cmd.Parameters.AddWithValue("@LoanStatus", "Pending"); // Set initial status to Pending

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Loan application submitted successfully with status: Pending.");
                }
            }
        }
        else
        {
            Console.WriteLine("Loan application cancelled.");
        }
    }
    

    public decimal calculateEMI(int loanId)
    {
        string query = "SELECT PrincipalAmount, InterestRate, LoanTerm FROM Loans WHERE LoanId = @LoanId";

        using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LoanId", loanId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    decimal principalAmount = reader.GetDecimal(0);
                    decimal annualInterestRate = reader.GetDecimal(1);
                    int loanTerm = reader.GetInt32(2);

                    // Calculate monthly interest rate
                    decimal monthlyInterestRate = annualInterestRate / 12 / 100;

                    // Calculate EMI using the formula
                    decimal emi = (principalAmount * monthlyInterestRate * (decimal)Math.Pow((double)(1 + monthlyInterestRate), loanTerm)) /
                                  ((decimal)Math.Pow((double)(1 + monthlyInterestRate), loanTerm) - 1);

                    return emi;
                }
                else
                {
                    throw new InvalidLoanException($"Loan with ID {loanId} not found.");
                }
            }
        }
    }

    
    public void loanRepayment(int loanId, int amount)
{
    Decimal payableEMI = calculateEMI(loanId);

    string selectQuery = "SELECT PrincipalAmount FROM Loans WHERE LoanId = @LoanId";

    using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
    {
        // Open the connection
        conn.Open();

        // Execute select query to retrieve PrincipalAmount
        using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
        {
            cmd.Parameters.AddWithValue("@LoanId", loanId);

            SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                // Loan not found, throw an exception or return
                throw new ArgumentException("Loan not found for the provided loan ID.");
            }

            int principalAmount = reader.GetInt32(0);

            // Close the reader after use
            reader.Close();

            // If amount is less than payableEMI, reject the payment
            if (amount < payableEMI)
            {
                throw new ArgumentException("Amount cannot be less than the payable EMI.");
            }
            else
            {
                Decimal numberOfEMIsPaid = amount / payableEMI;

                if (principalAmount % payableEMI == 0)
                {
                    Console.WriteLine("Your EMI payment is accepted.");
                    Console.WriteLine("Thanks for paying for {0} EMIs", numberOfEMIsPaid);
                }
                else
                {
                    Console.WriteLine("Partial payment accepted, excess will be refunded.");
                    Console.WriteLine("Thanks for paying for {0} EMIs", numberOfEMIsPaid);
                }

                // Updating the loan record to reflect new principal amount
                Decimal remainingPrincipal = principalAmount - (numberOfEMIsPaid * payableEMI);

                string updateQuery = "UPDATE Loans SET PrincipalAmount = @RemainingPrincipal WHERE LoanId = @LoanId";

                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@RemainingPrincipal", remainingPrincipal);
                    updateCmd.Parameters.AddWithValue("@LoanId", loanId);

                    // Execute the update query
                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Loan record updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to update loan record.");
                    }
                }
            }
        }
    }
}
    

    public void getAllLoan()
    {
        string query = "SELECT LoanId, PrincipalAmount, InterestRate, LoanTerm, LoanType, LoanStatus FROM Loans";

        using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            // conn is open
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();

                // Checking loans
                if (!reader.HasRows)
                {
                    Console.WriteLine("No loans found.");
                    return;
                }

                // Loop through the result and print loan details
                while (reader.Read())
                {
                    int loanId = reader.GetInt32(0);
                    decimal principalAmount = reader.GetDecimal(1);
                    decimal interestRate = reader.GetDecimal(2);
                    int loanTerm = reader.GetInt32(3);
                    string loanType = reader.GetString(4);
                    string loanStatus = reader.GetString(5);

                    Console.WriteLine($"Loan ID: {loanId}");
                    Console.WriteLine($"Principal Amount: {principalAmount}");
                    Console.WriteLine($"Interest Rate: {interestRate}%");
                    Console.WriteLine($"Loan Term: {loanTerm} months");
                    Console.WriteLine($"Loan Type: {loanType}");
                    Console.WriteLine($"Loan Status: {loanStatus}");
                    Console.WriteLine("------------");
                }
                
                // conn is closed
                reader.Close();
            }
        }
    }


    public void getLoanById(int loanId)
    {
        string query = "SELECT LoanId, PrincipalAmount, InterestRate, LoanTerm, LoanType, LoanStatus FROM Loans WHERE LoanId = @LoanId";

        using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LoanId", loanId);

                SqlDataReader reader = cmd.ExecuteReader();

                // Check if loan exists
                if (!reader.Read())
                {
                    throw new InvalidLoanException($"Loan with ID {loanId} not found.");
                }

                // Retrieve and print loan details
                int retrievedLoanId = reader.GetInt32(0);
                decimal principalAmount = reader.GetDecimal(1);
                decimal interestRate = reader.GetDecimal(2);
                int loanTerm = reader.GetInt32(3);
                string loanType = reader.GetString(4);
                string loanStatus = reader.GetString(5);

                Console.WriteLine($"Loan ID: {retrievedLoanId}");
                Console.WriteLine($"Principal Amount: {principalAmount}");
                Console.WriteLine($"Interest Rate: {interestRate}%");
                Console.WriteLine($"Loan Term: {loanTerm} months");
                Console.WriteLine($"Loan Type: {loanType}");
                Console.WriteLine($"Loan Status: {loanStatus}");

                reader.Close();
            }
        }
    }

// ######### all helper functions########################
    
private int GetCreditScore(int customerId)
    {
        string query = "SELECT CreditScore FROM CustomerCreditScores WHERE CustomerId = @CustomerId";
        
        using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception($"Credit score for Customer ID {customerId} not found.");
                }
            }
        }
    }


    private void UpdateLoanStatus(int loanId, string newStatus)
    {
        string updateQuery = "UPDATE Loans SET LoanStatus = @LoanStatus WHERE LoanId = @LoanId";

        using (var conn = PropertyUtils.PropertyUtil.getDBConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@LoanStatus", newStatus);
                cmd.Parameters.AddWithValue("@LoanId", loanId);

                cmd.ExecuteNonQuery();
            }
        }
    }
    //
    // private decimal CalculateEMI(decimal principalAmount, decimal annualInterestRate, int loanTerm)
    // {
    //     // Calculate monthly interest rate
    //     decimal monthlyInterestRate = annualInterestRate / 12 / 100;
    //
    //     // Calculate EMI using the formula
    //     decimal emi = (principalAmount * monthlyInterestRate * (decimal)Math.Pow((double)(1 + monthlyInterestRate), loanTerm)) /
    //                   ((decimal)Math.Pow((double)(1 + monthlyInterestRate), loanTerm) - 1);
    //
    //     return emi;
    // }
}

