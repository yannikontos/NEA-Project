using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

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
        //tables in my db are: 'Products', 'Customers', 'Orders', 'ProductsInOrders'
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
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-4."); 
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
                            Console.Clear();
                            Console.WriteLine("Goodbye! Exiting The Program Now...");
                            Thread.Sleep(2500);
                            Environment.Exit(0);
                            break;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) 
                {
                    Console.WriteLine("Invalid choice. Please enter integers in the range 1-4.");
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

                    if (CustomerInitialChoice >= 1 && CustomerInitialChoice <= 5)
                    {
                        decision = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-5.");
                    }

                    switch (CustomerInitialChoice)
                    {
                        case 1:
                            CheckCustomerStockPanel();
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
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Invalid choice. Please enter integers in the range 1-5.");
                }
                clearScreen++;
                if (clearScreen >= 5) { CustomerPage(); }

            }
        }
        public static void AdminPage()
        {
            Console.Clear();

            int AdminInitialChoice;
            bool decision = false;
            int clearScreen = 0;

            Console.WriteLine("stress test for the admin's page");
            Console.WriteLine("would you like to: ");
            Console.WriteLine("1. Check all products in the database ");
            Console.WriteLine("2. Check all customers in the database");
            Console.WriteLine("3. Check all customer's orders");
            Console.WriteLine("4. Check all products used in orders");
            Console.WriteLine("5. Exit to main menu");

            while (!decision)
            {
                try
                {
                    AdminInitialChoice = int.Parse(Console.ReadLine());

                    if (AdminInitialChoice >= 1 && AdminInitialChoice <= 3)
                    {
                        decision = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-5.");
                    }

                    switch (AdminInitialChoice)
                    {
                        case 1:
                            CheckAdminStockPanel();
                            break;
                        case 2:
                            DisplayCustomerAccountDetails();
                            break;                 
                        case 3:
                            ViewAllCustomerOrders();
                            break;                 
                        case 4:
                            ViewProductsInOrders();
                            break;
                        case 5:
                            MainMenu();
                            break;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Invalid choice. Please enter integers in the range 1-5.");
                }
                clearScreen++;
                if (clearScreen >= 5) { AdminPage(); }
            }
        }
        public static void CheckAdminStockPanel()
        {
            // this function is used to check to see if there are any products in stock, if so then will ask the user if they want to filter through the table 
         
            Console.Clear();
            Products products = new Products();
            SQLiteCommand sqlSelect = new SQLiteCommand("SELECT * FROM Products", conn);
            SQLiteDataReader reader; 
            reader = sqlSelect.ExecuteReader();
            Console.WriteLine("These are the currently listed Products in the database: \n");
            bool decision = false;
            char choice;
            int clearScreen = 0;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    bool DoProductsExist = true;
                    int productId = Convert.ToInt32(reader["ProductId"]);
                    string productName = reader["ProductName"].ToString();
                    string description = reader["Description"].ToString();
                    decimal price = Convert.ToDecimal(reader["Price"]);
                    int stockQuantity = Convert.ToInt32(reader["StockQuantity"]);

                    Console.WriteLine($" ProductId: {productId} \n ProductName: {productName} \n Description: {description}");

                    if (price > 50) { Console.ForegroundColor = ConsoleColor.Red; } else { Console.ForegroundColor = ConsoleColor.Green; }
                    Console.WriteLine($" Price: £{price}");

                    if (stockQuantity < 10) { Console.ForegroundColor = ConsoleColor.Red; } else { Console.ForegroundColor = ConsoleColor.Green; }
                    Console.WriteLine($" StockQuantity: {stockQuantity} \n");

                    Console.ResetColor();

                    Console.WriteLine("Would you like to add more products to the database? [Y/N]");

                    try
                    {
                        choice = Char.ToLower(char.Parse(Console.ReadLine()));
                        if (choice != 'y' || choice != 'n') { Console.WriteLine("Invalid choice. Please enter either [Y/N]"); } else { decision = true; }

                        switch (choice)
                        {
                            case 'y':
                                AddRecordsToTables(DoProductsExist, productId);
                                break;
                            case 'n':
                                AdminPage();
                                break;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter either [Y/N].");
                    }
                    AddRecordsToTables(DoProductsExist, productId);
                }
            }
            else
            {
                Console.WriteLine("There are no currently listed items in the 'Products' table, would you like to add some? [Y/N]");
                int productId = 0;
                bool DoProductsExist = false;

                while (!decision)
                {
                    try
                    {
                        choice = Char.ToLower(char.Parse(Console.ReadLine()));

                        if (choice != 'y' || choice != 'n') { Console.WriteLine("Invalid choice. Please enter either [Y/N]"); } else { decision = true; }

                        switch (choice) 
                        {
                            case 'y':
                                AddRecordsToTables(DoProductsExist, productId);
                                break;
                            case 'n':
                                AdminPage();
                                break;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter [Y/N]");
                    }
                    clearScreen++;
                    if (clearScreen >= 5) { CheckAdminStockPanel(); }
                }
            }

        }

        public static void AddRecordsToTables(bool DoProductsExist, int productId)
        {
            Console.Clear();
            string ProductName, Description;
            int StockQuantity;
            decimal Price;

            try
            {
                Console.WriteLine("Enter your desired product name");
                ProductName = Console.ReadLine();

                Console.WriteLine("Enter your description for your product");
                Description = Console.ReadLine();

                Console.WriteLine("Enter the stock amount for the product");
                StockQuantity = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter a price");
                Price = decimal.Parse(Console.ReadLine());

                int ProductId = DoProductsExist ? productId : 0; 

                SQLiteCommand createRecords = new SQLiteCommand("INSERT INTO Products(ProductId, ProductName, Description, Price, StockQuantity) VALUES ('" + ProductId + ", '" + ProductName + "', '" + Description + "', '" + Price + "', '" + StockQuantity + "')", conn);
                createRecords.ExecuteNonQuery();

                Console.WriteLine(ProductName + " " + "added to database");
                Console.WriteLine("Press enter to return to menu");
                Console.ReadKey();
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                Console.WriteLine("Invalid Choice. Please enter correct data type");
            }
        }
        public static void CheckCustomerStockPanel()
        {
            Console.WriteLine("list of items will be queried and displayed here");
        }
        public static void DisplayCustomerAccountDetails()
        {
            Console.WriteLine("accounts details for customers will be displayed here");
        }
        public static void ViewAllCustomerOrders()
        {

        }
        public static void ViewProductsInOrders()
        {

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
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Please submit the number '1' when ready to exit");
                }
                clearScreen++;
                if (clearScreen >= 5) { StartMenuInformation(); }
            }
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