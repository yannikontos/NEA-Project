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
using System.Security.Cryptography;

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
            Console.WriteLine("2. Check your basket");
            Console.WriteLine("3. Check your orders");
            Console.WriteLine("4. Exit to main menu");

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
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-4.");
                    }

                    switch (CustomerInitialChoice)
                    {
                        case 1:
                            CheckCustomerStockPanel();
                            break;
                        case 2:
                            CheckBasket();
                            break;
                        case 3:
                            CheckCustomerOrders();
                            break;
                        case 4:
                            MainMenu();
                            break;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Invalid choice. Please enter integers in the range 1-4.");
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
            Console.Clear();
            SQLiteCommand sqlSelect = new SQLiteCommand($"SELECT * FROM {tableName}", conn);
            SQLiteDataReader reader;
            reader = sqlSelect.ExecuteReader();
            bool decision = false;
            int clearScreen = 0;
            int itemId = 0;
            string tablePrimaryKey = "";

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
            Dictionary<string, Type> headerMaps = tableColumnHeaders[tableName];


            if (reader.HasRows)
            {
                bool DoRecordItemsExist = true;
                string outputText;


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

                        if (value.Key == "ProductId" || value.Key == "OrderId" || value.Key == "CustomerId" || value.Key == "ProductInOrderId") { itemId = (int)value.Value; tablePrimaryKey = value.Key; }

                        // value gives out the [record header name, actual value of the header i.e. ProductId: 0 | ProductName: shirt]
                        Console.WriteLine(outputText);
                        Console.ResetColor();
                    }

                    if (headerMaps.Count == columnValues.Count) { Console.WriteLine(); }
                }

                Console.WriteLine();
                Console.WriteLine("Would you like to: ");
                Console.WriteLine($"1. Add more {tableName} records to the table");
                Console.WriteLine($"2. Delete {tableName} records in the table");
                Console.WriteLine($"3. Filter through {tableName} based on ...");
                Console.WriteLine($"4. Filter through {tableName} based on ...");
                Console.WriteLine($"5. Filter through {tableName} based on ...");
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
                                AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues, headerMaps);
                                decision = true;
                                break;
                            case 2:
                                DeleteRecordsFromTables(tableName, itemId, tablePrimaryKey);
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
                                AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues, headerMaps);
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
        public static void AddRecordsToTables(bool DoRecordItemsExist, string tableName, int itemId, Dictionary<string, object> columnValues, Dictionary<string, Type> headerMaps)
        {
            Console.Clear();
            bool decision = false;
            Dictionary<string, object> NoRecordItems = new Dictionary<string, object>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            itemId = DoRecordItemsExist ? itemId + 1 : itemId;

            // add temp info into columnValues
            if (!DoRecordItemsExist)
            {
                foreach (var header in headerMaps)
                {
                    Type typeCheck = header.Value;

                    if (typeCheck == typeof(int)) { columnValues.Add(header.Key, 0); }
                    else if (typeCheck == typeof(decimal)) { columnValues.Add(header.Key, 0m); }
                    else { columnValues.Add(header.Key, ""); }
                }
            }

            // implement: regex for email address input, user names

            while (!decision)
            {
                try
                {
                    AddValuesToQuery(columnValues, itemId, parameters);

                    // command would like a little like: (ProductId, ProductName, Description, Price, StockQuantity) VALUES (@ProductId, @ProductName, @Description, @Price, @StockQuantity)
                    using (SQLiteCommand createRecords = new SQLiteCommand(conn))
                    {
                        string insertCommand = $"INSERT INTO {tableName} ({string.Join(", ", parameters.Keys)}) VALUES ({string.Join(", ", parameters.Keys.Select(key => "@" + key))})";
                        createRecords.CommandText = insertCommand;

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
        public static Dictionary<string, object> AddValuesToQuery(Dictionary<string, object> columnValues, int itemId, Dictionary<string, object> parameters) 
        {
            string output; 

            foreach (var value in columnValues)
            {
                output = value.Key.ToString();

                if (output == "ProductId" || output == "OrderId" || output == "CustomerId" || output == "ProductInOrderId") { Console.WriteLine($"{output}: {itemId}"); parameters.Add(output, itemId); }
                else if (output == "OrderDate")
                {
                    Console.WriteLine("enter your desired date: ");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    parameters.Add(output, date);
                }
                else if (output == "PhoneNumber") { ValidatePhoneNumber(parameters, value); }
                else if (output == "EmailAddress") { ValidateEmailAddress(parameters, value); }
                else
                {
                    Console.WriteLine($"Enter your desired {output}:");
                    string input = Console.ReadLine();
                    decimal validDecimal = 0;
                    bool containsAnInt = input.Any(char.IsDigit);
                    bool isInt = input.All(char.IsDigit);
                    bool isDecimal = decimal.TryParse(input, out validDecimal);
                    // for .Any method, 'a2' '2' returns true, 'a' returns false, whereas 'all' method returns false on everything but just ints, or is true on anything containing a char

                    if (containsAnInt && value.Value.GetType() == typeof(string) || value.Value.GetType() == typeof(decimal) && !isDecimal || value.Value.GetType() == typeof(int) && !isInt || input == "") { throw new FormatException(); }
                    else { parameters.Add(output, input); }
                }
            }
            return parameters;
        }
        public static string ValidateEmailAddress(Dictionary<string, object> parameters, KeyValuePair<string, object> value)
        {
            string userInput;
            string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";

            Console.WriteLine("enter your desired EmailAddress");
            userInput = Console.ReadLine();
            parameters[value.Key] = Regex.IsMatch(userInput, pattern) ? userInput : throw new FormatException();
            return userInput;
        }
        public static string ValidatePhoneNumber(Dictionary<string, object> parameters, KeyValuePair<string, object> value)
        {
            string userInput;
            string phonePattern = @"\s*(?:\+?(\d{1,3}))?([-. (]*(\d{3})[-. )]*)?((\d{3})[-. ]*(\d{2,4})(?:[-.x ]*(\d+))?)\s*";

            Console.WriteLine("enter your desired PhoneNumber: ");
            userInput = Console.ReadLine();
            parameters[value.Key] = Regex.IsMatch(userInput, phonePattern) ? userInput : throw new FormatException();
            return userInput;
        }
        public static void DeleteRecordsFromTables(string tableName, int itemId, string tablePrimaryKey)
        {
            int userInput;
            int clearScreen = 0;
            bool decision = false;
            Console.Clear();

            while (!decision)
            {
                try
                {
                    Console.WriteLine("Enter the item index you want to delete");
                    userInput = int.Parse(Console.ReadLine());

                    if (userInput <= itemId)
                    {
                        SQLiteCommand sqlDelete = new SQLiteCommand($"DELETE FROM {tableName} WHERE {tablePrimaryKey} = {userInput}", conn);
                        sqlDelete.ExecuteNonQuery();
                        Console.WriteLine($"Item index {userInput} has been deleted from {tableName}, enter any key to continue");
                        Console.ReadKey();

                        CheckAdminStockPanel(tableName);
                        decision = true;
                    }
                    else { throw new FormatException(); }

                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Invalid choice, Please enter a valid integer next time.");
                }
                clearScreen++;
                if (clearScreen >= 5) { DeleteRecordsFromTables(tableName, itemId, tablePrimaryKey); }
            }

        }
        public static void FilterItems(string whichFilter)
        {

        }
        public static void CheckCustomerStockPanel()
        {
            Console.WriteLine("list of items will be queried and displayed here");
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
