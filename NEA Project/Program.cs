using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace NEA_Project
{
    public class Products
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
    public class Customers
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
    public class Orders
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public decimal Total { get; set; }
        public int CustomerId { get; set; }
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

                    if (choice >= 1 && choice <= 4) { decision = true; }
                    else { Console.WriteLine("Invalid choice. Please enter integers in the range 1-4."); };

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
                            Thread.Sleep(2000);
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
            Console.WriteLine("1. Check all products in the database");
            Console.WriteLine("2. Check all customers in the database");
            Console.WriteLine("3. Check all customer's orders");
            Console.WriteLine("4. Check all products used in orders");
            Console.WriteLine("5. Exit to main menu");

            while (!decision)
            {
                try
                {
                    AdminInitialChoice = int.Parse(Console.ReadLine());
                    if (AdminInitialChoice < 1 || AdminInitialChoice > 5) { Console.WriteLine("Invalid choice. Please enter integers in the range 1-5."); }

                    switch (AdminInitialChoice)
                    {
                        case 1:
                            CheckAdminStockPanel("Products");
                            decision = true;
                            break;
                        case 2:
                            CheckAdminStockPanel("Customers");
                            decision = true;
                            break;
                        case 3:
                            CheckAdminStockPanel("Orders");
                            decision = true;
                            break;
                        case 4:
                            CheckAdminStockPanel("ProductsInOrders");
                            decision = true;
                            break;
                        case 5:
                            MainMenu();
                            decision = true;
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
        public static void CheckAdminStockPanel(string tableName)
        {
            // this function is used to check to see if there are any products in stock, if so then will ask the user if they want to filter through the table 

            Console.Clear();
            SQLiteCommand sqlSelect = new SQLiteCommand($"SELECT * FROM {tableName}", conn);
            SQLiteDataReader reader;
            reader = sqlSelect.ExecuteReader();
            bool decision = false;
            int clearScreen = 0;
            int itemId = 0;

            Dictionary<string, object> columnValues = new Dictionary<string, object>();

            Dictionary<string, Dictionary<string, Type>> tableColumnHeaders = new Dictionary<string, Dictionary<string, Type>>
            {
                {
                    "Products", new Dictionary<string, Type>
                    {
                        { "ProductId", typeof(int) },
                        { "ProductName", typeof(string) },
                        { "Description", typeof(string) },
                        { "Price", typeof(decimal) },
                        { "StockQuantity", typeof(int) }
                    }
                },
                {
                    "Customers", new Dictionary<string, Type>
                    {
                        { "CustomerId", typeof(int) },
                        { "FirstName", typeof(string) },
                        { "LastName", typeof(string) },
                        { "PhoneNumber", typeof(string) },
                        { "EmailAddress", typeof(string) }
                    }
                },
                {
                    "Orders", new Dictionary<string, Type>
                    {
                        { "OrderId", typeof(int) },
                        { "OrderDate", typeof(string) },
                        { "Total", typeof(decimal) },
                        { "CustomerId", typeof(int) }
                    }
                },
                {
                    "ProductsInOrders", new Dictionary<string, Type>
                    {
                        { "ProductInOrderId", typeof(int) },
                        { "ProductId", typeof(int) },
                        { "OrderId", typeof(int) },
                        { "Quantity", typeof(int) }
                    }
                }
            };

            Console.WriteLine($"These are the currently listed {tableName} in the database: \n");


            if (reader.HasRows)
            {
                Dictionary<string, Type> headerMaps = tableColumnHeaders[tableName];
                bool DoRecordItemsExist = true;
                string outputText = "";

                while (reader.Read())
                {
                    foreach (var headers in headerMaps)
                    {
                        string recordHeader = headers.Key;
                        Type recordType = headers.Value;

                        if (recordType == typeof(int))
                        {
                            columnValues[recordHeader] = Convert.ToInt32(reader[recordHeader]);
                        }
                        else if (recordType == typeof(string))
                        {
                            columnValues[recordHeader] = reader[recordHeader].ToString();
                        }
                        else if (recordType == typeof(decimal))
                        {
                            columnValues[recordHeader] = Convert.ToDecimal(reader[recordHeader]);
                        }
                        // reader[recordHeader] is the value, i.e. 0, shirt, nice looking shirt, £20.50, 20
                        // headers gives out the [record header name i.e. ProductId, ProductName and the data type i.e. string , int]
                    }

                    foreach (var value in columnValues)
                    {
                        outputText = Regex.Replace(value.ToString(), @"\[|\]", "").Replace(", ", ": ");

                        if (value.Key == "Price" && (decimal)value.Value >= 50) { outputText = $"Price: £{value.Value}"; Console.ForegroundColor = ConsoleColor.Red; }
                        else if (value.Key == "Price" && (decimal)value.Value < 50) { outputText = $"Price: £{value.Value}"; Console.ForegroundColor = ConsoleColor.Green; }

                        if (value.Key == "StockQuantity" && (int)value.Value >= 10) { outputText = $"StockQuantity: {value.Value}"; Console.ForegroundColor = ConsoleColor.Red; }
                        else if (value.Key == "stockQuantity" && (int)value.Value < 10) { outputText = $"StockQuantity: {value.Value}"; Console.ForegroundColor = ConsoleColor.Green; }

                        if (value.Key == "ProductId" || value.Key == "OrderId" || value.Key == "CustomerId" || value.Key == "ProductInOrderId") { itemId = (int)value.Value; }

                        // value gives out the [record header name, actual value of the header i.e. ProductId: 0 | ProductName: shirt]
                        Console.WriteLine(outputText);
                        Console.ResetColor();
                    }

                    if (headerMaps.Count == columnValues.Count) { Console.WriteLine(); }
                }

                Console.WriteLine();
                Console.WriteLine("Would you like to: ");
                Console.WriteLine($"1. Add more {tableName} records to the database");
                Console.WriteLine($"2. Delete {tableName} records in the database");
                Console.WriteLine($"3. Filter through {tableName} based on StockQuantity ascending");
                Console.WriteLine($"4. Filter through {tableName} based on ProductName ascending");
                Console.WriteLine($"5. Filter through {tableName} based on Price ascending");
                Console.WriteLine("6. Exit to menu");

                while (!decision)
                {
                    try
                    {
                        int choice;
                        choice = int.Parse(Console.ReadLine());

                        if (choice >= 1 && choice <= 6) { decision = true; }
                        else { Console.WriteLine("Invalid choice. Please enter integers in the range 1-6."); };

                        switch (choice)
                        {
                            case 1:
                                AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues);
                                decision = true;
                                break;
                            case 2:
                                DeleteRecordsFromTables();
                                decision = true;
                                break;
                            case 3:
                                FilterItems("StockQuantity");
                                decision = true;
                                break;
                            case 4:
                                FilterItems("ProductName");
                                decision = true;
                                break;
                            case 5:
                                FilterItems("Price");
                                decision = true;
                                break;
                            case 6:
                                AdminPage();
                                decision = true;
                                break;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-6.");
                    }
                    clearScreen++;
                    if (clearScreen >= 5) { CheckAdminStockPanel(tableName); }
                }
            }

            else
            {
                Console.WriteLine($"There are no currently listed items in the '{tableName}' table, would you like to add some? [Y/N]");
                bool DoRecordItemsExist = false;
                char choice;

                while (!decision)
                {
                    try
                    {
                        choice = Char.ToLower(char.Parse(Console.ReadLine()));
                        if (choice != 'y' || choice != 'n') { Console.WriteLine("Invalid choice. Please enter either [Y/N]"); }

                        switch (choice)
                        {
                            case 'y':
                                AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues);
                                decision = true;
                                break;
                            case 'n':
                                AdminPage();
                                decision = true;
                                break;
                        }
                    }

                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter either [Y/N]");
                    }
                    clearScreen++;
                    if (clearScreen >= 5) { CheckAdminStockPanel(tableName); }
                }
            }
        }
        public static void AddRecordsToTables(bool DoRecordItemsExist, string tableName, int itemId, Dictionary<string, object> columnValues)
        {
            Console.Clear();
            bool decision = false;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string output;

            itemId = DoRecordItemsExist ? itemId + 1 : itemId;

            while (!decision)
            {
                try
                {

                    foreach (var value in columnValues)
                    {
                        output = value.Key.ToString();
                        Type typeCheck = value.Value.GetType();

                        if (output == "ProductId" || output == "OrderId" || output == "CustomerId" || output == "ProductInOrderId")  { Console.WriteLine($"{output}: {itemId}"); parameters.Add(output, itemId); }
                        else
                        {
                            Console.WriteLine($"Enter your desired {output}:");
                            string input = Console.ReadLine();
                            bool containsInt = input.Any(char.IsDigit);

                            // a string input type cannot be converted into an int, but if it can and is valid, then tell the user to re enter information
                            // if a user inputs a string, and the string can convert into an int, then ask the user to reinput the number
                            // detects ints as 2s, 2, will return true

                            //checks if the input contains an int when the column header is a string, throws an error if so
                            if (containsInt && value.Value.GetType() == typeof(string)) { throw new FormatException(); }
                            //else { parameters[value.Key] = input; };


                            // check if a column header is an int and if the user input is an int, if so add to the params dict, else throw exception
                            //if (input && value.Value.GetType() == typeof(int)) { throw new FormatException(); }
                            //else { throw new FormatException(); };

                            // check if a column header is an decimal and if the user input is a decimal, if so add to the params dict, else throw exception
                            //if (containsInt && value.Value.GetType() == typeof(decimal)) { throw new FormatException(); }
                            //else { throw new FormatException(); };





                            //if (input.GetType() == typeof(int) && value.Key.GetType() != typeof(int))
                            //{
                            //    throw new Exception();
                            //}
                            //else { Console.WriteLine("hello"); parameters[value.Key] = Convert.ToInt32(input); }

                            //if (input.GetType() == typeof(int))
                            //{
                            //    parameters[value.Key] = Convert.ToInt32(input);
                            //}
                            //else if (typeCheck == typeof(string))
                            //{
                            //    parameters[value.Key] = input.ToString();
                            //}
                            //else if (typeCheck == typeof(decimal))
                            //{
                            //    parameters[value.Key] = Convert.ToDecimal(input);
                            //}
                        }
                    }

                    foreach (var param in parameters)
                    {
                        Console.WriteLine("Key: " + param.Key.GetType());
                        Console.WriteLine("Value: " + param.Value.GetType());
                    }

                    // for some reason this foreach loop isnt being read when trying to input a new record into the db, and thus its values cannot be used as parameters through the INSERT Query
                    // input sanitisation

                    using (SQLiteCommand createRecords = new SQLiteCommand(conn))
                    {
                        string insertCommand = $"INSERT INTO {tableName} ({string.Join(", ", parameters.Keys)}) VALUES ({string.Join(", ", parameters.Keys.Select(key => "@" + key))})";
                        createRecords.CommandText = insertCommand;
                        // command would like a little like: (ProductId, ProductName, Description, Price, StockQuantity) VALUES (@ProductId, @ProductName, @Description, @Price, @StockQuantity)

                        foreach (var param in parameters)
                        {
                            createRecords.Parameters.AddWithValue("@" + param.Key, param.Value);
                        }
                        createRecords.ExecuteNonQuery();
                    }

                    Console.WriteLine();
                    Console.WriteLine("Item successfully added to the database");
                    Console.WriteLine("Press any key to return to the menu");
                    Console.ReadKey();
                    decision = true;
                    AdminPage();
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid Choice. Please enter correct data types");
                    parameters.Clear();
                }
            }
        }
        public static void FilterItems(string whichFilter)
        {

        }
        public static void DeleteRecordsFromTables()
        {

        }
        public static void CheckCustomerStockPanel()
        {
            Console.WriteLine("list of items will be queried and displayed here");
        }
        public static void DisplayCustomerAccountDetails()
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
        public static void StartMenuInformation()
        {
            Console.Clear();
            Console.WriteLine("Option 1 is used for testing out the customer's perspective of the business; which can be useful for " +
                "checking products in stock, checking customer details, basket and orders.");
            Console.WriteLine("\n");
            Console.WriteLine("Option 2 is used for testing out the customer's perspective of the business; which can be useful for " +
                "checking all products in the database, checking all customers and their details, checking all customer orders and " +
                "finally, being able to add or remove products and customers.");
            Console.WriteLine("\n press any key when you are ready to exit.");
            Console.ReadKey();
            MainMenu();
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


