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
            List<int> availableTableIds = new List<int>();

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

                        if (value.Key == "Total") { outputText = $"Total: £{value.Value}"; }

                        if (value.Key == "ProductId" && tableName == "Products" || value.Key == "OrderId" && tableName == "Orders" || value.Key == "CustomerId" && tableName == "Customers" || value.Key == "ProductInOrderId" && tableName == "ProductsInOrders") { tablePrimaryKey = value.Key; itemId = (int)value.Value; availableTableIds.Add(itemId); }

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
                if (tableName == "Orders") { Console.WriteLine($"3. Fetch customer order information based on provided OrderId"); }
                Console.WriteLine($"4. Filter through {tableName} based on ...");
                Console.WriteLine("5. Exit to menu");

                while (!decision)
                {
                    try
                    {
                        int choice;
                        choice = int.Parse(Console.ReadLine());

                        if (choice >= 1 && choice <= 5) { decision = true; }
                        else { Console.WriteLine("Invalid choice. Please enter integers in the range 1-5."); };

                        switch (choice)
                        {
                            case 1:
                                AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues, headerMaps);
                                decision = true;
                                break;
                            case 2:
                                DeleteRecordsFromTables(tableName, availableTableIds, tablePrimaryKey);
                                decision = true;
                                break;
                            case 3:
                                GetCustomerOrdersWithProducts(tableName);
                                //fix this
                                decision = true;
                                break;
                            case 4:
                                FilterItems("ProductName");
                                decision = true;
                                break;
                            case 5:
                                AdminPage();
                                decision = true;
                                break;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-5.");
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
                //dicts or hashtables?
            }
        }
        public static void AddRecordsToTables(bool DoRecordItemsExist, string tableName, int itemId, Dictionary<string, object> columnValues, Dictionary<string, Type> headerMaps)
        {
            Console.Clear();
            bool decision = false;
            Dictionary<string, object> NoRecordItems = new Dictionary<string, object>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<int> customerIds = new List<int>();
            List<int> productIds = new List<int>();
            List<int> orderIds = new List<int>();

            itemId = DoRecordItemsExist ? itemId + 1 : itemId;

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

            while (!decision)
            {
                try
                {
                    if (tableName == "Orders" || tableName == "ProductsInOrders") { ValidateKeys(tableName, customerIds, productIds, orderIds); }
                    AddValuesToQuery(columnValues, itemId, parameters, tableName, customerIds, productIds, orderIds);


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
        public static void ValidateKeys(string tableName, List<int> customerIds, List<int> productIds, List<int> orderIds)
        {
            SQLiteCommand sqlCustomers = new SQLiteCommand($"SELECT * FROM Customers", conn);
            SQLiteCommand sqlProducts = new SQLiteCommand($"SELECT * FROM Products", conn);
            SQLiteCommand sqlOrders = new SQLiteCommand($"SELECT * FROM Orders", conn);
            SQLiteDataReader reader1 = sqlCustomers.ExecuteReader();
            SQLiteDataReader reader2 = sqlProducts.ExecuteReader();
            SQLiteDataReader reader3 = sqlOrders.ExecuteReader();
            string outputText = "";


            var errorOutputs = new Dictionary<string, string>()
            {
                {"Orders", "Customers" },
                {"ProductsInOrders", "Products And Orders" }
            };

            if (tableName == "Orders" && !reader1.HasRows || tableName == "ProductsInOrders" && !reader2.HasRows || tableName == "ProductsInOrders" && !reader3.HasRows)
            {
                outputText += errorOutputs[tableName];

                Console.WriteLine($"The {outputText} table has no entries; in order to add entries to the {tableName} table you will need to input data into {outputText} \nPress any key to return to the menu");
                Console.ReadKey();
                AdminPage();
            }

            if (tableName == "Orders")
            {
                customerIds.Clear();
                while (reader1.Read()) { customerIds.Add(Convert.ToInt32(reader1["CustomerId"])); }
            }
            else
            {
                productIds.Clear();
                orderIds.Clear();

                while (reader2.Read()) { productIds.Add(Convert.ToInt32(reader2["ProductId"])); }
                while (reader3.Read()) { orderIds.Add(Convert.ToInt32(reader3["OrderId"])); }
            }
        }
        public static Dictionary<string, object> AddValuesToQuery(Dictionary<string, object> columnValues, int itemId, Dictionary<string, object> parameters, string tableName, List<int> customerIds, List<int> productIds, List<int> orderIds)
        {
            string output;

            foreach (var value in columnValues)
            {
                output = value.Key.ToString();

                if (output == "ProductId" && tableName == "Products" || output == "OrderId" && tableName == "Orders" || output == "CustomerId" && tableName == "Customers" || output == "ProductInOrderId" && tableName == "ProductsInOrders") { Console.WriteLine($"{output}: {itemId}"); parameters.Add(output, itemId); }
                else if (output == "OrderDate")
                {
                    Console.WriteLine("enter your desired date: ");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    parameters.Add(output, date);
                }
                else if (output == "PhoneNumber") { ValidatePhoneNumber(parameters, value); }
                else if (output == "EmailAddress") { ValidateEmailAddress(parameters, value); }
                else if (output == "Description") { ValidateDescription(parameters, value); }
                else if (output == "CustomerId" && tableName == "Orders") { ValidateCustomerId(customerIds, parameters); }
                else if (output == "ProductId" && tableName == "ProductsInOrders") { ValidateSecondaryKeys(productIds, orderIds, parameters, output); }
                else if (output == "OrderId" && tableName == "ProductsInOrders") { ValidateSecondaryKeys(productIds, orderIds, parameters, output); }
                else
                {
                    Console.WriteLine($"Enter your desired {output}:");
                    string input = Console.ReadLine();
                    decimal validDecimal = 0;
                    bool containsAnInt = input.Any(char.IsDigit);
                    bool isInt = input.All(char.IsDigit);
                    bool isDecimal = decimal.TryParse(input, out validDecimal);
                    // for .Any method, 'a2' '2' returns true, 'a' returns false, whereas 'all' method returns false on everything but only ints, or is true on anything containing a char

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
        public static string ValidateDescription(Dictionary<string, object> parameters, KeyValuePair<string, object> value)
        {
            string userInput;

            Console.WriteLine("enter your desired Description: ");
            userInput = Console.ReadLine();
            bool isInt = userInput.All(char.IsDigit);
            parameters[value.Key] = !isInt ? userInput : throw new FormatException();

            return userInput;
        }
        public static Dictionary<string, object> ValidateCustomerId(List<int> customerIds, Dictionary<string, object> parameters)
        {
            bool isAssigned = false;
            SQLiteCommand sqlOrders = new SQLiteCommand($"SELECT * FROM Orders", conn);
            SQLiteDataReader orderReader = sqlOrders.ExecuteReader();
            List<int> checkOrderIds = new List<int>();

            while (orderReader.Read())
            {
                checkOrderIds.Add(Convert.ToInt32(orderReader["CustomerId"]));
            }

            if (customerIds.Count == checkOrderIds.Count) 
            {
                Console.Clear();
                Console.WriteLine("All the possible CustomerId's have been assigned already, thus you must add more customers into the database in order to have more orders \nPress any key to continue ");
                Console.ReadKey();
                AdminPage();
            }

            while (!isAssigned)
            {
                Console.WriteLine("Assign a customerId to this order");
                int userInputtedCustomerId = int.Parse(Console.ReadLine());

                if (!customerIds.Contains(userInputtedCustomerId) || checkOrderIds.Contains(userInputtedCustomerId)) { Console.Clear(); Console.WriteLine("The number inputted is not an available CustomerId"); }
                else
                {
                    parameters.Add("CustomerId", userInputtedCustomerId);
                    isAssigned = true;
                }
            }

            return parameters;
        }
        public static Dictionary<string, object> ValidateSecondaryKeys(List<int> productIds, List<int> orderIds, Dictionary<string, object> parameters, string output)
        {
            bool isAssigned = false;

            while (!isAssigned && output == "ProductId")
            {
                Console.WriteLine("Enter your desired ProductId");
                int productId = int.Parse(Console.ReadLine());
                parameters.Add("ProductId", productId);

                isAssigned = true;
            }

            while (!isAssigned && output == "OrderId")
            {
                Console.WriteLine("Enter your desired asd OrderId");
                int orderId = int.Parse(Console.ReadLine());
                parameters.Add("OrderId", orderId);

                isAssigned = true;
            }

            //for quantity: take the productId then get the stockQuantity, then ensure in ProductsInOrders that the user cannot select more than the current stockQuantity available
            //ensure validity of productId and orderId. possibly make it less verbose
            return parameters;
        }
        public static void DeleteRecordsFromTables(string tableName, List<int> availableTableIds, string tablePrimaryKey)
        {
            Console.Clear();
            int userInput;
            int clearScreen = 0;
            bool decision = false;

            while (!decision)
            {
                try
                {
                    Console.WriteLine("Enter the item index you want to delete");
                    userInput = int.Parse(Console.ReadLine());

                    if (availableTableIds.Contains(userInput))
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
                if (clearScreen >= 5) { DeleteRecordsFromTables(tableName, availableTableIds, tablePrimaryKey); }
            }

        }
        public static void GetCustomerOrdersWithProducts(string tableName)
        {
            Console.Clear();
            int inputId;
            int clearScreen = 0;
            bool decision = false;

            while (!decision)
            {
                try
                {
                    Console.WriteLine("enter the index for the desired orderID");
                    inputId = int.Parse(Console.ReadLine());

                    string sql = "SELECT Orders.OrderId, Orders.OrderDate, Products.ProductName, Products.Price, " +
                                    "Customers.FirstName || ' ' || Customers.LastName AS CustomerFullName " +
                                    "FROM ProductsInOrders " +
                                    "INNER JOIN Products ON ProductsInOrders.ProductId = Products.ProductId " +
                                    "INNER JOIN Orders ON ProductsInOrders.OrderId = Orders.OrderId " +
                                    "INNER JOIN Customers ON Orders.CustomerId = Customers.CustomerId " +
                                    "WHERE ProductsInOrders.OrderId = @OrderId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", inputId);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int orderId = reader.GetInt32(0);
                                string orderDate = reader.GetString(1);
                                string productName = reader.GetString(2);
                                decimal price = reader.GetDecimal(3);
                                string fullName = reader.GetString(4);
                                Console.WriteLine($"Order ID: {orderId}, \nName: {fullName}, \nProduct Ordered: {productName}, \nPrice: {price:C}, \nDate: {orderDate}");
                            }
                        }
                    }

                    Console.WriteLine("\nitem found, press any key to return");
                    Console.ReadKey();
                    CheckAdminStockPanel(tableName);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("enter a valid integer index.");
                }
                clearScreen++;
                if (clearScreen >= 5) { GetCustomerOrdersWithProducts(tableName); }

                // fetch an orderId from Orders, then use that to find the ProductId from ProductsInOrders, then use that index to find the productName and price from Products table
            }
        }

        //public static decimal CalculateTotalOrderPrice(int orderId)
        //{
        //    string sql = "SELECT SUM(Products.Price * ProductsInOrders.Quantity) " +
        //                 "FROM ProductsInOrders " +
        //                 "INNER JOIN Products ON ProductsInOrders.ProductId = Products.ProductId " +
        //                 "WHERE ProductsInOrders.OrderId = @OrderId";

        //    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
        //    {
        //        cmd.Parameters.AddWithValue("@OrderId", orderId);
        //        object result = cmd.ExecuteScalar();
        //      return Convert.ToDecimal(result);
        //    }
        //}
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





