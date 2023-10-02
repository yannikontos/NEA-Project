using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace NEA_Project
{
    public class Products 
    {
        public int ProductId { get; set; }
        public int StockQuantity { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
    public class Customers 
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }   
    public class Orders
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int OrderDate { get; set; }
        public string Total { get; set; }
    }   
    public class ProductsInOrders
    {
        public int ProductInOrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
    }

    internal class Program
    {
        public static SQLiteConnection conn = new SQLiteConnection("Data Source=databaseNEA.db;Version=3;New=True;Compress=True;");
        static void Main(string[] args)
        {
            conn.Open();
            MainMenu();


            conn.Close();

            Console.ReadKey();
        }

        public static void MakeLoginDecision()
        {
            int choice;
            bool decision = false;
            int clearScreen = 0;

            while (!decision)
            {
                try
                {
                    choice = int.Parse(Console.ReadLine());

                    if (choice >= 1 && choice <= 4)
                    {
                        decision = true;
                    }
                    else 
                    { 
                        Console.WriteLine("Invalid choice. Please enter numbers in the range 1-4."); 
                    };

                    switch (choice)
                    {
                        case 1:
                            CustomerPage();
                            break;
                        case 2:
                            AdminPage();
                            break;
                        case 3:
                            StartMenuInformation();
                            break;
                        case 4:
                            Console.WriteLine("Goodbye!");
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid integer data type");
                }
                clearScreen++;
                if (clearScreen >= 5) { MainMenu(); }
            }
            Console.ReadKey();
        }
        public static void CustomerPage()
        {
            int CustomerInitialChoice;
            bool decision = false;
            int clearScreen = 0;

            Console.Clear();
            Console.WriteLine("Welcome to the customer's page");
            Console.WriteLine("would you like to: ");
            Console.WriteLine("1. See a list of available products in stock");
            Console.WriteLine("2. See your account details");
            Console.WriteLine("3. Check your basket");
            Console.WriteLine("4. Check your orders");
            Console.WriteLine("5. Exit to main menu");

            while (!decision)
            {
                try
                {
                    CustomerInitialChoice = int.Parse(Console.ReadLine());

                    if (CustomerInitialChoice >= 1 && CustomerInitialChoice <= 4)
                    {
                        decision = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter numbers in the range 1-4.");
                    }

                    switch (CustomerInitialChoice)
                    {
                        case 1:
                            CheckStock();
                            break;
                        case 2:
                            DisplayCustomerAccountDetails();
                            break;
                        case 3:
                            CheckBasket();
                            break;
                        case 4:
                            CheckCustomerOrders();
                            break;
                        case 5:
                            MainMenu();
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid integer data type");
                }
                clearScreen++;
                if (clearScreen >= 5) { CustomerPage(); }

            }
        }
        public static void AdminPage()
        {
            Console.Clear();
            Console.WriteLine("stress test for the admin's page");
            Console.WriteLine("would you like to: ");

        }
        public static void StartMenuInformation()
        {
            int ExitCommand;
            bool decision = false;
            int clearScreen = 0;

            Console.Clear();
            Console.WriteLine("Option 1 is used for testing out the customer's perspective of the business; which can be useful for " +
                "checking products in stock, checking customer details, basket and orders.");
            Console.WriteLine("\n");
            Console.WriteLine("Option 2 is used for testing out the customer's perspective of the business; which can be useful for " +
                "checking all products in the database, checking all customers and their details, checking all customer orders and " +
                "finally, being able to add or remove products and customers.");
            Console.WriteLine("\n press '1' when you are ready to exit.");

            while (!decision)
            {
                try
                {
                    ExitCommand = int.Parse(Console.ReadLine());

                    if (ExitCommand == 1)
                    {
                        decision = true;
                        MainMenu();
                    }
                    else
                    {
                        Console.WriteLine("Enter '1' to exit");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid integer data type");
                }
                clearScreen++;
                if (clearScreen >= 5) { StartMenuInformation(); }
            }
        }
        public static void CheckStock()
        {
            Console.WriteLine("list of items will be queried and displayed here");
        }
        public static void DisplayCustomerAccountDetails()
        {
            Console.WriteLine("accounts details for customers will be displayed here");
        }
        public static void CheckBasket()
        {
            Console.WriteLine("list of items in the customer's basket will be added here");
        }
        public static void CheckCustomerOrders()
        {
            string CustomerQueryName;

            Console.WriteLine("what is your name?");
            CustomerQueryName = Console.ReadLine();
        }
        public static void ProductPage()
        {
            Console.Clear();
            Console.WriteLine("Would you like to: ");
            Console.WriteLine();
            Console.WriteLine("1. View all products listed in the database,");
            Console.WriteLine("2. Sort by product name in ascending order (a-z),");
            Console.WriteLine("3. Sort by product price from highest costing to lowest.");

        }
        public static void OrderPage()
        {
            Console.Clear();
            Console.WriteLine("customer's orders will appear here");
        }
        public static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Your Database Information, Choose An Option Below:");
            Console.WriteLine();
            Console.WriteLine("1. Customer's Page");
            Console.WriteLine("2. Admin's Page");
            Console.WriteLine("3. Information for both options");
            Console.WriteLine();
            Console.WriteLine("4. Exit");
            MakeLoginDecision();
        }
    }
}