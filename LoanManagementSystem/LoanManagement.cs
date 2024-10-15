using System;
using LoanManagementSystem.BussnissLogicLayer.Services;
using LoanManagementSystem.Entity;
using LoanManagementSystem.ExceptionHandling;

public class LoanManagement
{
    
    public static void Main(string[] args)
    {
        LoanServices _loanServices = new LoanServices();

        while (true)
        {
            Console.WriteLine("\n--- Loan Management System ---");
            Console.WriteLine("1. Apply for a Loan");
            Console.WriteLine("2. Get All Loans");
            Console.WriteLine("3. Get Loan by ID");
            Console.WriteLine("4. Make Loan Repayment");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter Customer ID: ");
                    int customerId = int.Parse(Console.ReadLine());

                    Console.Write("Enter Customer Name: ");
                    string name = Console.ReadLine();

                    Console.Write("Enter Customer Email: ");
                    string email = Console.ReadLine();

                    Console.Write("Enter Customer Phone Number: ");
                    string phoneNumber = Console.ReadLine();

                    Console.Write("Enter Customer Address: ");
                    string address = Console.ReadLine();

                    Console.Write("Enter Customer Credit Score: ");
                    int creditScore = int.Parse(Console.ReadLine());

                    Customer customer = new Customer(customerId, name, email, phoneNumber, address, creditScore);

                    Console.Write("Enter Principal Amount: ");
                    decimal principalAmount = decimal.Parse(Console.ReadLine());

                    Console.Write("Enter Interest Rate (Annual): ");
                    decimal interestRate = decimal.Parse(Console.ReadLine());

                    Console.Write("Enter Loan Term (in months): ");
                    int loanTerm = int.Parse(Console.ReadLine());

                    Console.Write("Enter Loan Type (HomeLoan/CarLoan): ");
                    string loanType = Console.ReadLine();

                    Loan newLoan = new Loan(0, customer, principalAmount, interestRate, loanTerm, loanType);

                    _loanServices.applyLoan(newLoan);
                    break;
                
                
                
                case "2":
                    Console.Write("Enter Loan ID: ");
                    int loanId = int.Parse(Console.ReadLine());

                    try
                    {
                        _loanServices.getAllLoan();                    }
                    catch (InvalidLoanException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                    break;
                case "3":
                    Console.Write("Enter Loan ID: ");
                    int getLoanByloanId = int.Parse(Console.ReadLine());

                    try
                    {
                        _loanServices.getLoanById(getLoanByloanId);                    }
                    catch (InvalidLoanException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                    break;
                case "4":
                    Console.Write("Enter Loan ID for repayment: ");
                    int loanRepaymentId = int.Parse(Console.ReadLine());

                    Console.Write("Enter repayment amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());

                    try
                    {
                        _loanServices.loanRepayment(loanRepaymentId, amount);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                    break;
                case "5":
                    Console.WriteLine("Exiting the system. Goodbye!");
                    return; 
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }


}
