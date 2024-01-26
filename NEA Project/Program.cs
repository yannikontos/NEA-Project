
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
using System.Diagnostics.Eventing.Reader;
using System.Collections;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Data.SqlTypes;

namespace NEA_Project
{
    internal class Program
    {
        public static SQLiteConnection conn = new SQLiteConnection("Data Source=databaseNEA.db;Version=3;New=True;Compress=True;");
        static void Main(string[] args)
        {
            conn.Open();
            MainMenu();

            conn.Close();
            Console.ReadLine();
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


                    if (choice >= 1 && choice <= 2) { decision = true; }
                    else { Console.WriteLine("Invalid choice. Please enter integers in the range 1-2."); };


                    switch (choice)
                    {
                        case 1:
                            AdminPage();
                            break;
                        case 2:
                            Console.Clear();
                            Console.WriteLine("Goodbye! Exiting The Program Now...");
                            Thread.Sleep(2000);
                            Environment.Exit(0);
                            break;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Invalid choice. Please enter integers in the range 1-2.");
                }
                clearScreen++;
                if (clearScreen >= 3) { MainMenu(); }
            }
            Console.ReadKey();
        } //fine don't touch
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
                    else { decision = true; }

                    switch (AdminInitialChoice)
                    {
                        case 1:
                            CheckAdminStockPanel("Products");
                            break;
                        case 2:
                            CheckAdminStockPanel("Customers");
                            break;
                        case 3:
                            CheckAdminStockPanel("Orders");
                            break;
                        case 4:
                            CheckAdminStockPanel("ProductsInOrders");
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
                if (clearScreen >= 3) { AdminPage(); }
            }
        } //fine don't touch
        public static void CheckAdminStockPanel(string tableName)
        {
            Console.Clear();
            SQLiteCommand sqlSelect = new SQLiteCommand($"SELECT * FROM {tableName}", conn);
            SQLiteDataReader reader = sqlSelect.ExecuteReader();

            bool decision = false;
            int clearScreen = 0;
            int itemId = 0;
            string tablePrimaryKey = "";
            List<int> availableTableIds = new List<int>();


            Dictionary<string, object> columnValues = new Dictionary<string, object>();
            Dictionary<string, Dictionary<string, Type>> getDatabaseColumnHeaders = GetColumnHeaders();
            Console.WriteLine($"These are the currently listed {tableName} in the database: \n");
            Dictionary<string, Type> columnHeadersMapped = getDatabaseColumnHeaders[tableName];


            if (reader.HasRows)
            {
                bool DoRecordItemsExist = true;

                while (reader.Read())
                {
                    columnValues.Clear();

                    foreach (var headers in columnHeadersMapped)
                    {
                        string recordHeader = headers.Key;
                        Type recordType = headers.Value;

                        if (recordType == typeof(int)) { columnValues.Add(recordHeader, Convert.ToInt32(reader[recordHeader])); }
                        else if (recordType == typeof(string)) { columnValues.Add(recordHeader, reader[recordHeader].ToString()); }
                        else if (recordType == typeof(decimal)) { columnValues.Add(recordHeader, Convert.ToDecimal(reader[recordHeader])); }
                        // reader[recordHeader] is the value, i.e. 0, shirt, nice looking shirt, £20.50, 20
                        // headers gives out the [record header name i.e. ProductId, ProductName and the data type i.e. string , int]
                    }

                    foreach (var value in columnValues)
                    {
                        string columnHeader = value.Key;
                        object columnValue = value.Value;

                        if (columnHeader == "ProductId" && tableName == "Products" || columnHeader == "OrderId" && tableName == "Orders" || columnHeader == "CustomerId" && tableName == "Customers" || columnHeader == "ProductInOrderId" && tableName == "ProductsInOrders") { tablePrimaryKey = columnHeader; itemId = (int)columnValue; availableTableIds.Add(itemId); }
                        if (columnHeader == "Price") { Console.WriteLine($"{columnHeader}: £{columnValue}"); }
                        else { Console.WriteLine($"{columnHeader}: {columnValue}"); }

                    }

                    if (columnHeadersMapped.Count == columnValues.Count) { Console.WriteLine(); }
                }

                Console.WriteLine();
                Console.WriteLine("Would you like to: ");
                Console.WriteLine("1. Add, Update or Delete Values");
                Console.WriteLine("2. View Overall Database Analytics");
                Console.WriteLine("3. Filter Results");
                Console.WriteLine("4. Exit to menu");


                while (!decision)
                {
                    try
                    {
                        int choice;
                        choice = int.Parse(Console.ReadLine());

                        if (choice >= 1 && choice <= 4) { decision = true; }
                        else { Console.WriteLine("Invalid choice. Please enter integers in the range 1-4."); };

                        switch (choice)
                        {
                            case 1:
                                ChooseCRUDDecision(tableName, DoRecordItemsExist, itemId, columnValues, columnHeadersMapped, availableTableIds, tablePrimaryKey);
                                break;
                            case 2:
                                ChooseAnalytics(tableName);
                                break;
                            case 3:
                                FilterTable(tableName);
                                break;
                            case 4:
                                AdminPage();
                                break;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter integers in the range 1-4.");
                    }
                    clearScreen++;
                    if (clearScreen >= 3) { CheckAdminStockPanel(tableName); }
                }
            }


            else
            {
                Console.WriteLine($"There are no currently listed items in the '{tableName}' table, would you like to add some? [Y/N]");
                bool DoRecordItemsExist = false;

                while (!decision)
                {
                    try
                    {
                        char choice = Char.ToLower(char.Parse(Console.ReadLine()));
                        if (choice != 'y' || choice != 'n') { Console.WriteLine("Invalid choice. Please enter either [Y/N]"); }
                        else { decision = true; }

                        switch (choice)
                        {
                            case 'y':
                                AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues, columnHeadersMapped, tablePrimaryKey);
                                break;
                            case 'n':
                                AdminPage();
                                break;
                        }
                    }


                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid choice. Please enter either [Y/N]");
                    }
                    clearScreen++;
                    if (clearScreen >= 3) { CheckAdminStockPanel(tableName); }
                }
            }
        } // fine don't touch mostly
        public static Dictionary<string, Dictionary<string, Type>> GetColumnHeaders()
        {
            Dictionary<string, Dictionary<string, Type>> getDatabaseColumnHeaders = new Dictionary<string, Dictionary<string, Type>>
            {
                {
                    "Products", new Dictionary<string, Type>
                    {
                        { "ProductId", 0.GetType() },
                        { "ProductName", "".GetType() },
                        { "Description", "".GetType() },
                        { "Price", 0m.GetType() },
                        { "StockQuantity", 0.GetType() }
                    }
                },
                {
                    "Customers", new Dictionary<string, Type>
                    {
                        { "CustomerId", 0.GetType() },
                        { "FirstName", "".GetType() },
                        { "LastName", "".GetType() },
                        { "PhoneNumber", "".GetType() },
                        { "EmailAddress", "".GetType() }
                    }
                },
                {
                    "Orders", new Dictionary<string, Type>
                    {
                        { "OrderId", 0.GetType() },
                        { "OrderDate", "".GetType() },
                        { "CustomerId", 0.GetType() }
                    }
                },
                {
                    "ProductsInOrders", new Dictionary<string, Type>
                    {
                        { "ProductInOrderId", 0.GetType() },
                        { "ProductId", 0.GetType() },
                        { "OrderId", 0.GetType() },
                        { "Quantity", 0.GetType() }
                    }
                }
            };
            // initialised a nested dictionary with the keys of it being the column headers and also the values of it inferring their types to the contained dictionary

            return getDatabaseColumnHeaders;
        } // fine don't touch
        public static void ChooseCRUDDecision(string tableName, bool DoRecordItemsExist, int itemId, Dictionary<string, object> columnValues, Dictionary<string, Type> columnHeadersMapped, List<int> availableTableIds, string tablePrimaryKey)
        {
            int clearScreen = 0;
            bool decision = false;

            Console.Clear();
            Console.WriteLine("Would you like to: ");
            Console.WriteLine($"1. Add more {tableName} records to the table");
            Console.WriteLine($"2. Update the records in the {tableName} table");
            Console.WriteLine($"3. Delete {tableName} records in the table");
            Console.WriteLine($"4. Exit to menu");

            while (!decision)
            {
                try
                {
                    int choice = int.Parse(Console.ReadLine());

                    if (choice >= 1 && choice <= 4) { decision = true; }
                    else { Console.WriteLine("Invalid choice. Please enter either numbers within the range of 1-4"); }

                    switch (choice)
                    {
                        case 1:
                            AddRecordsToTables(DoRecordItemsExist, tableName, itemId, columnValues, columnHeadersMapped, tablePrimaryKey);
                            break;
                        case 2:
                            UpdateRecordsFromTables(tableName, columnValues, tablePrimaryKey);
                            break;
                        case 3:
                            DeleteRecordsFromTables(tableName, availableTableIds, tablePrimaryKey);
                            break;
                        case 4:
                            CheckAdminStockPanel(tableName);
                            break;
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Invalid choice. Please enter either numbers within the range of 1-4");
                }
                clearScreen++;
                if (clearScreen >= 3) { ChooseCRUDDecision(tableName, DoRecordItemsExist, itemId, columnValues, columnHeadersMapped, availableTableIds, tablePrimaryKey); }
            }
        } //fine don't touch
        public static void ChooseAnalytics(string tableName)
        {
            Console.Clear();
            bool decision = false;
            int clearScreen = 0;

            Console.WriteLine("Would you like to: ");
            Console.WriteLine("1. Get customer order details with inputted OrderId");
            Console.WriteLine("2. Get most expensive customer orders (ascending / descending)");
            Console.WriteLine("3. Get the average customer order expenditure");
            Console.WriteLine("4. Get the average quantity of items ordered");
            Console.WriteLine($"5. Exit to menu");

            while (!decision)
            {
                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    if (choice < 1 || choice > 5) { Console.WriteLine("Invalid input, please try again"); }
                    else { decision = true; }

                    switch (choice)
                    {
                        case 1:
                            CheckRows(tableName);
                            GetCustomerOrdersWithProducts(tableName);
                            break;
                        case 2:
                            CheckRows(tableName);
                            CalculateMostExpensiveOrder(tableName);
                            break;
                        case 3:
                            CheckRows(tableName);
                            GetAverageCustomerSpending(tableName);
                            break;
                        case 4:
                            CheckRows(tableName);
                            GetAverageQuantityOfItems(tableName);
                            break;
                        case 5:
                            CheckAdminStockPanel(tableName);
                            break;
                    }

                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) { Console.WriteLine("Invalid input, please try again"); }
                clearScreen++;
                if (clearScreen >= 3) { ChooseAnalytics(tableName); }
            }
        } //fine don't touch
        private delegate void ExecuteFilteringFunctions();
        public static void FilterTable(string tableName)
        {
            Console.Clear();
            bool makeDecision = false;
            int clearScreen = 0;
            string determineResult = "";

            Console.WriteLine($"Filter {tableName} items below :\n");

            Dictionary<string, List<string>> filterMessages = new Dictionary<string, List<string>>
            {
                {
                    "Products", new List<string>
                    {
                        {"1. Filter by Price in ascending / descending order"},
                        {"2. Filter by ProductName in alphabetical order ascending / descending"},
                        {"3. Filter by StockQuantity in ascending / descending order"},
                        {"4. Exit"},
                    }
                },
                {
                    "Customers", new List<string>
                    {
                        {"1. Filter by a customer's first name in ascending / descending order"},
                        {"2. Filter by a customer's last name in ascending / descending order"},
                        {"3. Exit"},
                    }
                },
                {
                    "Orders", new List<string>
                    {
                        {"1. Filter by order date (latest -> oldest)"},
                        {"2. Exit"}
                    }
                },
                {
                    "ProductsInOrders", new List<string>
                    {
                        {"1. Filter by quantity of items in ascending / descending order"},
                        {"2. Exit"}
                    }
                }
            };

            List<string> getOptions = filterMessages[tableName];

            Dictionary<string, ExecuteFilteringFunctions> filterMethods = new Dictionary<string, ExecuteFilteringFunctions>
            {
                {"1Products", () => FilterInputtedTables(tableName, "Price") },
                {"2Products", () => FilterInputtedTables(tableName, "ProductName") },
                {"3Products", () => FilterInputtedTables(tableName, "StockQuantity") },
                {"1Customers", () => FilterInputtedTables(tableName, "FirstName") },
                {"2Customers", () => FilterInputtedTables(tableName, "LastName") },
                {"1Orders", () => FilterInputtedTables(tableName, "OrderDate") },
                {"1ProductsInOrders", () => FilterInputtedTables(tableName, "Quantity") }
            };


            foreach (var item in getOptions)
            {
                Console.WriteLine(item);
            }

            while (!makeDecision)
            {
                try
                {
                    int choice = int.Parse(Console.ReadLine());

                    if (choice < 1 || choice > getOptions.Count) { Console.WriteLine($"Incorrect input, ensure your input is within 1-{getOptions.Count}"); }
                    else if (choice == getOptions.Count)
                    {
                        CheckAdminStockPanel(tableName);
                    }
                    else
                    {
                        makeDecision = true;
                        determineResult = choice.ToString() + tableName;
                        filterMethods[determineResult]();
                    }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) { Console.WriteLine($"Incorrect input, ensure your input is within 1-{getOptions.Count}"); }
                clearScreen++;
                if (clearScreen >= 3) { FilterTable(tableName); }
            }
        } // mostly fine
        public static void AddRecordsToTables(bool DoRecordItemsExist, string tableName, int itemId, Dictionary<string, object> columnValues, Dictionary<string, Type> columnHeadersMapped, string tablePrimaryKey)
        {
            Console.Clear();
            bool decision = false;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<int> customerIds = new List<int>();
            List<int> productIds = new List<int>();
            List<int> orderIds = new List<int>();
            HashSet<int> currentlyExistingIds = new HashSet<int>();
            List<int> missingIds = new List<int>();

            itemId = DoRecordItemsExist ? itemId + 1 : itemId;

            if (DoRecordItemsExist)
            {
                SQLiteCommand checkIds = new SQLiteCommand($"SELECT {tablePrimaryKey} FROM {tableName}", conn);
                SQLiteDataReader ids = checkIds.ExecuteReader();
                while (ids.Read()) { currentlyExistingIds.Add(Convert.ToInt32(ids[tablePrimaryKey])); }
            };

            for (int i = 0; i < itemId; i++) { if (!currentlyExistingIds.Contains(i)) { missingIds.Add(i); } }

            if (missingIds.Count > 0)
            {
                itemId = missingIds.Min();
                missingIds.Remove(itemId);
            }

            if (!DoRecordItemsExist)
            {
                foreach (var header in columnHeadersMapped)
                {
                    string columnHeader = header.Key;
                    Type typeCheck = header.Value;

                    if (typeCheck == typeof(int)) { columnValues.Add(columnHeader, 0); }
                    else if (typeCheck == typeof(decimal)) { columnValues.Add(columnHeader, 0m); }
                    else { columnValues.Add(columnHeader, ""); }
                }
            }

            while (!decision)
            {
                try
                {
                    if (tableName == "Orders" || tableName == "ProductsInOrders") { ValidateKeys(tableName, customerIds, productIds, orderIds); }
                    AddValuesToQuery(columnValues, itemId, parameters, tableName, customerIds, productIds, orderIds);

                    string headerList = String.Join(", ", parameters.Keys);
                    string headerParameters = String.Join(", :", parameters.Keys);

                    SQLiteCommand createRecords = new SQLiteCommand($"INSERT INTO {tableName} ({headerList}) VALUES (:{headerParameters})", conn);
                    foreach (var param in parameters) { createRecords.Parameters.AddWithValue($":{param.Key}", param.Value); };

                    createRecords.ExecuteNonQuery();

                    Console.WriteLine();
                    Console.WriteLine("Item successfully added to the database");
                    Console.WriteLine("Press any key to return to the menu");
                    Console.ReadKey();
                    decision = true;
                    CheckAdminStockPanel(tableName);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid Choice. Please enter correct data types");
                    parameters.Clear();
                }
            }
        } // fine don't touch
        public static void ValidateKeys(string tableName, List<int> customerIds, List<int> productIds, List<int> orderIds)
        {
            SQLiteCommand sqlCustomers = new SQLiteCommand($"SELECT * FROM Customers", conn);
            SQLiteCommand sqlProducts = new SQLiteCommand($"SELECT * FROM Products", conn);
            SQLiteCommand sqlOrders = new SQLiteCommand($"SELECT * FROM Orders", conn);
            SQLiteDataReader fetchCustomerId = sqlCustomers.ExecuteReader();
            SQLiteDataReader fetchProductId = sqlProducts.ExecuteReader();
            SQLiteDataReader fetchOrderId = sqlOrders.ExecuteReader();
            string outputText = "";


            var errorOutputs = new Dictionary<string, string>()
            {
                {"Orders", "Customers" },
                {"ProductsInOrders", "Products And Orders" }
            };


            if (tableName == "Orders" && !fetchCustomerId.HasRows || tableName == "ProductsInOrders" && !fetchProductId.HasRows || tableName == "ProductsInOrders" && !fetchOrderId.HasRows)
            {
                outputText += errorOutputs[tableName];

                Console.WriteLine($"The {outputText} table has no entries; in order to add entries to the {tableName} table you will need to input data into {outputText} \nPress any key to return to the menu");
                Console.ReadKey();
                CheckAdminStockPanel(tableName);
            }


            if (tableName == "Orders")
            {
                customerIds.Clear();
                while (fetchCustomerId.Read()) { customerIds.Add(Convert.ToInt32(fetchCustomerId["CustomerId"])); }
            }
            else
            {
                productIds.Clear();
                orderIds.Clear();

                while (fetchProductId.Read()) { productIds.Add(Convert.ToInt32(fetchProductId["ProductId"])); }
                while (fetchOrderId.Read()) { orderIds.Add(Convert.ToInt32(fetchOrderId["OrderId"])); }
            }
        }//fine don't touch
        public static Dictionary<string, object> AddValuesToQuery(Dictionary<string, object> columnValues, int itemId, Dictionary<string, object> parameters, string tableName, List<int> customerIds, List<int> productIds, List<int> orderIds)
        {
            string whichFunction = "AddValues";
            string tablePrimaryKey = "";

            foreach (var value in columnValues)
            {
                string output = value.Key.ToString();
                object columnValue = value.Value;

                if (output == "ProductId" && tableName == "Products" || output == "OrderId" && tableName == "Orders" || output == "CustomerId" && tableName == "Customers" || output == "ProductInOrderId" && tableName == "ProductsInOrders") { Console.WriteLine($"{output}: {itemId}"); parameters.Add(output, itemId); tablePrimaryKey = output; }
                else
                {
                    switch (output + tableName)
                    {
                        case "OrderDateOrders":
                            Console.WriteLine("enter your desired date: ");
                            DateTime date = DateTime.Parse(Console.ReadLine());
                            parameters.Add(output, date);
                            break;
                        case "PhoneNumberCustomers":
                            ValidatePhoneNumber(parameters, value, whichFunction, tableName, tablePrimaryKey, itemId);
                            break;
                        case "EmailAddressCustomers":
                            ValidateEmailAddress(parameters, value, whichFunction, null);
                            break;
                        case "CustomerIdOrders":
                            ValidateCustomerId(tableName, customerIds, parameters, whichFunction, 0);
                            break;
                        case "ProductIdProductsInOrders":
                            ValidateSecondaryKeys(tableName, productIds, orderIds, parameters, output);
                            break;
                        case "OrderIdProductsInOrders":
                            ValidateSecondaryKeys(tableName, productIds, orderIds, parameters, output);
                            break;
                        case "QuantityProductsInOrders":
                            ValidateQuantity(parameters, value, whichFunction, null, tableName);
                            break;
                        default:
                            GetUserInputForColumnHeaders(output, parameters, columnValue);
                            break;
                    }
                }
            }
            return parameters;
        } // fine don't touch
        public static string ValidateEmailAddress(Dictionary<string, object> parameters, KeyValuePair<string, object> value, string whichFunction, SQLiteCommand updateCommand)
        {
            string userInput = "";
            string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            bool patternMatched = false;

            while (!patternMatched)
            {
                try
                {
                    Console.WriteLine("enter your desired EmailAddress");
                    userInput = Console.ReadLine();
                    bool isMatch = Regex.IsMatch(userInput, pattern);

                    if (whichFunction == "AddValues" && isMatch) { parameters[value.Key] = userInput; patternMatched = true; }
                    else if (whichFunction == "UpdateRecords" && isMatch)
                    {
                        updateCommand.Parameters.AddWithValue($"@{value.Key}", userInput);
                        updateCommand.CommandText += $"{value.Key} = @{value.Key}, ";
                        patternMatched = true;
                    }
                    else { Console.Clear(); throw new FormatException(); }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) { Console.WriteLine("Incorrect Email Address, Please try again"); }
            }

            return userInput;
        } // needs rework
        public static string ValidatePhoneNumber(Dictionary<string, object> parameters, KeyValuePair<string, object> value, string whichFunction, string tableName, string tablePrimaryKey, int primaryKeyValue)
        {
            string userInput = "";
            string phonePattern = @"\s*(?:\+?(\d{1,3}))?([-. (]*(\d{3})[-. )]*)?((\d{3})[-. ]*(\d{2,4})(?:[-.x ]*(\d+))?)\s*";
            bool patternMatched = false;

            while (!patternMatched)
            {
                try
                {
                    Console.WriteLine("enter your desired PhoneNumber: ");
                    userInput = Console.ReadLine();
                    bool isMatch = Regex.IsMatch(userInput, phonePattern);

                    if (whichFunction == "AddValues" && isMatch) { parameters[value.Key] = userInput; patternMatched = true; } // fix the parameters add 
                    else if (whichFunction == "UpdateRecords" && isMatch)
                    {
                        SQLiteCommand updatePhoneNumber = new SQLiteCommand($@"UPDATE {tableName}
                        SET PhoneNumber = '{userInput}'
                        WHERE {tablePrimaryKey} = {primaryKeyValue}", conn);
                        updatePhoneNumber.ExecuteNonQuery();
                        patternMatched = true;
                    }
                    else { Console.Clear(); throw new FormatException(); }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) { Console.WriteLine("Incorrect Phone Number, Please try again"); }
            }
            return userInput;
        } // needs rework
        public static Dictionary<string, object> ValidateCustomerId(string tableName, List<int> customerIds, Dictionary<string, object> parameters, string chooseValidationMethod, int orderId)
        {
            bool isAssigned = false;
            SQLiteCommand sqlOrders = new SQLiteCommand($"SELECT * FROM Orders", conn);
            SQLiteDataReader orderReader = sqlOrders.ExecuteReader();
            List<int> usedCustomerIds = new List<int>();

            while (orderReader.Read())
            {
                usedCustomerIds.Add(Convert.ToInt32(orderReader["CustomerId"]));
            }

            if (!isAssigned)
            {
                while (!isAssigned)
                {
                    Console.WriteLine("Assign a customerId to this order");
                    int userInputtedCustomerId = int.Parse(Console.ReadLine());

                    if (!customerIds.Contains(userInputtedCustomerId)) { Console.Clear(); Console.WriteLine("The number inputted is not an available CustomerId"); }
                    else if (chooseValidationMethod == "Update")
                    {
                        SQLiteCommand updateCustomerId = new SQLiteCommand($@"UPDATE {tableName} 
                        SET CustomerId = {userInputtedCustomerId}
                        WHERE OrderId = {orderId};", conn);
                        updateCustomerId.ExecuteNonQuery();
                        isAssigned = true;
                    }
                    else
                    {
                        parameters.Add("CustomerId", userInputtedCustomerId);
                        isAssigned = true;
                    }
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("All the possible CustomerId's have been assigned already, thus you must add more customers into the database in order to have more orders \nPress any key to continue ");
                Console.ReadKey();
                CheckAdminStockPanel(tableName);
            }
            return parameters;
        }  // fine dont touch
        public static Dictionary<string, object> ValidateSecondaryKeys(string tableName, List<int> productIds, List<int> orderIds, Dictionary<string, object> parameters, string output)
        {
            bool isAssigned = false;
            SQLiteCommand sqlProductsInOrders = new SQLiteCommand($"SELECT * FROM ProductsInOrders", conn);
            SQLiteDataReader productsInOrdersReader = sqlProductsInOrders.ExecuteReader();
            SQLiteCommand selectProductStock = new SQLiteCommand($"SELECT ProductId FROM Products WHERE StockQuantity = 0", conn);
            SQLiteDataReader stockReader = selectProductStock.ExecuteReader();
            List<int> usedProductIds = new List<int>();
            List<int> usedOrderIds = new List<int>();
            List<int> productsOutOfStock = new List<int>();
            string outputtedErrorMessage;

            List<bool> isOrderIdUsed = new List<bool>();
            bool entryCheck = false;


            while (productsInOrdersReader.Read())
            {
                usedProductIds.Add(Convert.ToInt32(productsInOrdersReader["ProductId"]));
                usedOrderIds.Add(Convert.ToInt32(productsInOrdersReader["OrderId"]));
            }


            while (stockReader.Read()) { productsOutOfStock.Add(Convert.ToInt32(stockReader["ProductId"])); }


            foreach (int availableOrderIds in orderIds)
            {
                isOrderIdUsed.Add(usedOrderIds.Contains(availableOrderIds));
                entryCheck = isOrderIdUsed.All(ids => ids == true);
                // use the All method to determine true or false values
                // if there is a false entry, then allow the user to enter in more records as there are spare orderIds
            }

            var errorMessage = new Dictionary<string, string>()
            {
                {"NoStock", "The item corresponding to your inputted Product ID is currently out of stock"},
                {"NoID", "The item corresponding to your inputted Product ID does not exist in the database"},

                {"ItemID", "The inputted OrderID is already being used in this table"},
                {"IDDoesNotExist", "The inputted OrderID does not exist"},
            };


            if (!entryCheck)
            {
                while (!isAssigned && output == "ProductId")
                {
                    Console.WriteLine("Enter your desired ProductId");
                    int inputtedProductId = int.Parse(Console.ReadLine());

                    outputtedErrorMessage = productsOutOfStock.Contains(inputtedProductId) ? errorMessage["NoStock"] : errorMessage["NoID"];

                    if (!productIds.Contains(inputtedProductId) || productsOutOfStock.Contains(inputtedProductId)) { Console.Clear(); Console.WriteLine(outputtedErrorMessage); }
                    else { parameters.Add("ProductId", inputtedProductId); isAssigned = true; }
                }


                while (!isAssigned && output == "OrderId")
                {
                    Console.WriteLine("Enter your desired OrderId");
                    int inputtedOrderId = int.Parse(Console.ReadLine());

                    outputtedErrorMessage = usedOrderIds.Contains(inputtedOrderId) ? errorMessage["ItemID"] : errorMessage["IDDoesNotExist"];

                    if (usedOrderIds.Contains(inputtedOrderId) || !orderIds.Contains(inputtedOrderId)) { Console.Clear(); Console.WriteLine(outputtedErrorMessage); }
                    else { parameters.Add("OrderId", inputtedOrderId); isAssigned = true; }
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("All of the current orders have been fulfilled, please input more order records before adding any to this. \nPress any key to continue");
                Console.ReadKey();
                CheckAdminStockPanel(tableName);
            }
            return parameters;
        }// fine dont touch
        public static int ValidateQuantity(Dictionary<string, object> parameters, KeyValuePair<string, object> value, string whichFunction, SQLiteCommand updateCommand, string tableName)
        {
            bool isAssigned = false;
            int inputtedQuantity = 0;
            int getProductId = 0;
            int availableQuantity = 0;
            // SQLiteCommand updateCommand = new SQLiteCommand($"UPDATE {tableName} SET ", conn);

            foreach (var item in parameters) { if (item.Key == "ProductId") { getProductId = Convert.ToInt32(item.Value); } }

            SQLiteCommand sqlStockQuantity = new SQLiteCommand($"SELECT StockQuantity FROM Products WHERE ProductId = {getProductId}", conn);
            SQLiteDataReader stockQuantity = sqlStockQuantity.ExecuteReader();


            while (stockQuantity.Read()) { availableQuantity = Convert.ToInt32(stockQuantity["stockQuantity"]); }

            while (!isAssigned && value.Key == "Quantity")
            {
                Console.WriteLine("Enter your desired Quantity");
                inputtedQuantity = int.Parse(Console.ReadLine());


                if (inputtedQuantity <= availableQuantity && inputtedQuantity >= 1 && whichFunction == "AddValues")
                {
                    parameters.Add("Quantity", inputtedQuantity);
                    SQLiteCommand updateStockAvailability = new SQLiteCommand($"UPDATE Products SET StockQuantity = {availableQuantity - inputtedQuantity} WHERE ProductId = {getProductId}", conn);
                    updateStockAvailability.ExecuteNonQuery();
                    isAssigned = true;
                }
                else if (whichFunction == "AddValues") { Console.Clear(); Console.WriteLine("That amount of stock is not available, check if the product is in stock if needed"); }
                else
                {
                    Console.WriteLine(whichFunction);
                    //updateCommand = new SQLiteCommand($"UPDATE {tableName} SET @{value.Value} = {inputtedQuantity}", conn);

                    updateCommand.Parameters.AddWithValue($"@{value.Value}", inputtedQuantity);
                    updateCommand.CommandText += $"{value.Value} = @{value.Value}, ";
                    isAssigned = true;
                } // when in an update command, 
            }
            return inputtedQuantity; //fix (working on this)
        }
        public static void GetUserInputForColumnHeaders(string output, Dictionary<string, object> parameters, object columnValue)
        {
            Console.WriteLine($"Enter your desired {output}:");
            string getUserInput = Console.ReadLine();
            decimal validDecimal = 0;
            bool stringContainsInt = getUserInput.Any(char.IsDigit);
            bool isStringAnInt = getUserInput.All(char.IsDigit);
            bool isStringDecimal = decimal.TryParse(getUserInput, out validDecimal);
            //for the .Any method, 'a2' '2' returns true, 'a' returns false, whereas 'all' method returns false on everything but only ints, or is true on anything containing a char. simply does input validation

            if (stringContainsInt && columnValue.GetType() == typeof(string) || columnValue.GetType() == typeof(decimal) && !isStringDecimal || columnValue.GetType() == typeof(int) && !isStringAnInt || string.IsNullOrWhiteSpace(getUserInput)) { throw new FormatException(); }
            else { parameters.Add(output, getUserInput); }
        } // completely fine, don't touch
        public static void UpdateRecordsFromTables(string tableName, Dictionary<string, object> columnValues, string tablePrimaryKey)
        {
            //less verbose - DO THIS
            Console.Clear();

            bool isValidInt = false;
            bool validateInputs = false;
            int userInputtedId = 0;
            List<int> validIds = new List<int>();
            List<int> customerIds = new List<int>();
            string whichFunction = "UpdateRecords";

            SQLiteCommand sqlCustomers = new SQLiteCommand($"SELECT * FROM Customers", conn);
            SQLiteCommand getvalidIds = new SQLiteCommand($"SELECT {tablePrimaryKey} FROM {tableName}", conn);
            SQLiteDataReader getCustomers = sqlCustomers.ExecuteReader();
            SQLiteDataReader invalidIdReader = getvalidIds.ExecuteReader();

            while (invalidIdReader.Read()) { validIds.Add(Convert.ToInt32(invalidIdReader[tablePrimaryKey])); }
            while (getCustomers.Read()) { customerIds.Add(Convert.ToInt32(getCustomers["CustomerId"])); }

            while (!isValidInt)
            {
                try
                {
                    Console.WriteLine("Enter the item id you would like to update");
                    userInputtedId = int.Parse(Console.ReadLine());
                    userInputtedId = validIds.Contains(userInputtedId) ? userInputtedId : throw new FormatException();
                    isValidInt = true;
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) { Console.Clear(); Console.WriteLine("enter a valid integer index."); }
            }

            SQLiteCommand updateCommand = new SQLiteCommand($"UPDATE {tableName} SET ", conn);

            while (!validateInputs)
            {
                foreach (var values in columnValues)
                {
                    string columnHeaders = values.Key.ToString();
                    object columnValue = values.Value;

                    if (columnHeaders == tablePrimaryKey) { continue; }

                    try
                    {
                        if (columnHeaders == "OrderDate")
                        {
                            Console.WriteLine("enter your desired date: (format of: mm/dd/yyyy)");
                            DateTime date = DateTime.Parse(Console.ReadLine());
                            string formattedDate = date.ToString("MM/dd/yyyy");
                            // this type conversion is necessary so that date can be plugged into the update command properly, perhaps need to reference this

                            SQLiteCommand updateOrderDate = new SQLiteCommand($@"UPDATE {tableName} 
                            SET {columnHeaders} = '{formattedDate}'
                            WHERE OrderId = {userInputtedId};", conn);
                            updateOrderDate.ExecuteNonQuery();
                            // add an individual separate command here to update it individually
                        }
                        else if (columnHeaders == "PhoneNumber") { ValidatePhoneNumber(columnValues, values, whichFunction, tableName, tablePrimaryKey, userInputtedId); }
                        else if (columnHeaders == "EmailAddress") { ValidateEmailAddress(columnValues, values, whichFunction, updateCommand); }
                        else if (columnHeaders == "Quantity" && tableName == "ProductsInOrders") { ValidateQuantity(columnValues, values, whichFunction, updateCommand, tableName); }
                        else if (columnHeaders == "CustomerId" && tableName == "Orders") { ValidateCustomerId(tableName, customerIds, null, "Update", userInputtedId); }
                        else
                        {
                            Console.WriteLine($"Enter your desired {columnHeaders}:"); // edit this when coming back ------------------------ fix the date registration as its being caught as a format exception, add each sqlCommand to each function 
                            string input = Console.ReadLine();
                            decimal validDecimal = 0;
                            bool containsAnInt = input.Any(char.IsDigit);
                            bool isInt = input.All(char.IsDigit);
                            bool isDecimal = decimal.TryParse(input, out validDecimal);

                            if (containsAnInt && columnValue.GetType() == typeof(string) || columnValue.GetType() == typeof(decimal) && !isDecimal || columnValue.GetType() == typeof(int) && !isInt || input == "") { UpdateRecordsFromTables(tableName, columnValues, tablePrimaryKey); }

                            SQLiteCommand secondaryKeyCommands = new SQLiteCommand($@"UPDATE {tableName}
                            SET {columnHeaders} = '{input}'
                            WHERE {tablePrimaryKey} = {userInputtedId}
                            ", conn);
                            secondaryKeyCommands.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException) { UpdateRecordsFromTables(tableName, columnValues, tablePrimaryKey); }
                }
                validateInputs = true;
            }

            Console.WriteLine("Item updated, press any key to continue");
            Console.ReadKey();
            CheckAdminStockPanel(tableName);
        }
        public static void DeleteRecordsFromTables(string tableName, List<int> availableTableIds, string tablePrimaryKey)
        {
            Console.Clear();
            int getUserInput;
            int clearScreen = 0;
            bool decision = false;

            while (!decision)
            {
                try
                {
                    Console.WriteLine("Enter the item index you want to delete");
                    getUserInput = int.Parse(Console.ReadLine());


                    if (availableTableIds.Contains(getUserInput))
                    {
                        SQLiteCommand sqlDelete = new SQLiteCommand($"DELETE FROM {tableName} WHERE {tablePrimaryKey} = {getUserInput}", conn);
                        sqlDelete.ExecuteNonQuery();
                        Console.WriteLine($"Item index {getUserInput} has been deleted from {tableName}, enter any key to continue");
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
                if (clearScreen >= 3) { DeleteRecordsFromTables(tableName, availableTableIds, tablePrimaryKey); }
            }
        } // completely fine, don't touch
        public static void CheckRows(string tableName)
        {
            SQLiteCommand selectCustomers = new SQLiteCommand("SELECT * FROM Customers", conn);
            SQLiteCommand selectOrders = new SQLiteCommand("SELECT * FROM Orders", conn);
            SQLiteCommand selectProducts = new SQLiteCommand("SELECT * FROM Products", conn);
            SQLiteCommand selectProductsInOrders = new SQLiteCommand("SELECT * FROM ProductsInOrders", conn);

            SQLiteDataReader customers = selectCustomers.ExecuteReader();
            SQLiteDataReader orders = selectOrders.ExecuteReader();
            SQLiteDataReader products = selectProducts.ExecuteReader();
            SQLiteDataReader productsInOrders = selectProductsInOrders.ExecuteReader();
            Console.Clear();

            if (!customers.HasRows || !orders.HasRows || !products.HasRows || !productsInOrders.HasRows) { Console.WriteLine("Before using this function you must input data into each table, press any key to continue"); Console.ReadKey(); CheckAdminStockPanel(tableName); }
        } // completely fine, don't touch
        public static void GetCustomerOrdersWithProducts(string tableName)
        {
            SQLiteCommand getSqlOrders = new SQLiteCommand($"SELECT * FROM Orders", conn);
            SQLiteDataReader sqlOrderReader = getSqlOrders.ExecuteReader();
            List<int> availableOrderIds = new List<int>();
            int userInputId;
            bool decision = false;

            while (sqlOrderReader.Read()) { availableOrderIds.Add(Convert.ToInt32(sqlOrderReader["OrderId"])); }

            while (!decision)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("enter a valid index for your desired OrderId");
                    userInputId = int.Parse(Console.ReadLine());

                    if (availableOrderIds.Contains(userInputId))
                    {
                        SQLiteCommand selectOrderDetails = new SQLiteCommand($@"SELECT ProductsInOrders.OrderId, Orders.OrderDate, Customers.FirstName,
                        Customers.LastName, Products.ProductName, Products.Price, ProductsInOrders.Quantity, SUM(ProductsInOrders.Quantity * Products.Price) AS totalSpending
                        FROM ProductsInOrders
                        INNER JOIN Orders 
                        ON ProductsInOrders.OrderId = Orders.OrderId
                        INNER JOIN Customers 
                        ON Orders.CustomerId = Customers.CustomerId
                        INNER JOIN Products 
                        ON ProductsInOrders.ProductId = Products.ProductId
                        WHERE ProductsInOrders.OrderId = {userInputId}; ", conn);


                        SQLiteDataReader getOrderDetails = selectOrderDetails.ExecuteReader();

                        while (getOrderDetails.Read())
                        {
                            Console.Clear();
                            int orderDetailsId = getOrderDetails.GetInt32(0);
                            DateTime orderDetailsDate = getOrderDetails.GetDateTime(1);
                            string firstName = getOrderDetails.GetString(2);
                            string lastName = getOrderDetails.GetString(3);
                            string productName = getOrderDetails.GetString(4);
                            decimal productPrice = getOrderDetails.GetDecimal(5);
                            int productQuantity = getOrderDetails.GetInt32(6);
                            decimal totalSpending = getOrderDetails.GetDecimal(7);


                            Console.WriteLine("Item found: \n");
                            Console.WriteLine($"Order ID: {orderDetailsId}, \nOrder Date: {orderDetailsDate}, \nCustomer Name: {firstName} {lastName}, \nProduct Ordered: {productName}, \nProduct Price: £{productPrice}, \nQuantity: {productQuantity}, \nTotal: £{totalSpending}\n");
                            Console.WriteLine("Press Any Key To Return");
                            Console.ReadKey();
                            CheckAdminStockPanel(tableName);
                            decision = true;
                        }
                    }
                    else { Console.Clear(); Console.WriteLine("Incorrect OrderId"); }
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("enter a valid integer index.");
                }
            }
        } //  touch
        public static void CalculateMostExpensiveOrder(string tableName)
        {
            Console.Clear();
            string orderMethod = GetOrderMethod();

            SQLiteCommand fetchOrders = new SQLiteCommand($@"SELECT Customers.CustomerId, Customers.FirstName, Customers.LastName, SUM(Products.Price * ProductsInOrders.Quantity) AS total
            FROM Customers
            LEFT JOIN Orders 
            ON Customers.CustomerId = Orders.CustomerId
            LEFT JOIN ProductsInOrders 
            ON Orders.OrderId = ProductsInOrders.OrderId
            LEFT JOIN Products 
            ON ProductsInOrders.ProductId = Products.ProductId
            GROUP BY Customers.CustomerId, Customers.FirstName, Customers.LastName
            ORDER BY total {orderMethod} LIMIT 3", conn);

            SQLiteDataReader orderReader = fetchOrders.ExecuteReader();
            Console.Clear();

            while (orderReader.Read())
            {
                int fetchedCustomerId = orderReader.GetInt32(0);
                string fetchedFirstName = orderReader.GetString(1);
                string fetchedLastName = orderReader.GetString(2);
                decimal total = orderReader.GetDecimal(3);

                Console.WriteLine($"The Customer With CustomerId Of: {fetchedCustomerId}, {fetchedFirstName} {fetchedLastName}, has a total spending of £{total}");
            }

            Console.WriteLine("\nPress Any Key To Continue");
            Console.ReadKey();
            CheckAdminStockPanel(tableName);
        } // touch
        public static void GetAverageCustomerSpending(string tableName)
        {
            SQLiteCommand getCustomerOrders = new SQLiteCommand($@"SELECT ProductsInOrders.ProductId, AVG(ProductsInOrders.Quantity * Products.Price) AS averageCustomerSpending
            FROM ProductsInOrders 
            INNER JOIN Products 
            ON ProductsInOrders.ProductId = Products.ProductId ", conn);
            SQLiteDataReader spendingReader = getCustomerOrders.ExecuteReader();
            Console.Clear();

            while (spendingReader.Read())
            {
                decimal averageCustomerSpending = spendingReader.GetDecimal(1);
                Console.WriteLine($"The average amount a customer spends on their orders are: £{Math.Round(averageCustomerSpending)}");
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            CheckAdminStockPanel(tableName);
        } // maybe look
        public static void GetAverageQuantityOfItems(string tableName)
        {
            SQLiteCommand getAverageQuantity = new SQLiteCommand($@"SELECT AVG(Quantity) AS averageQuantity FROM ProductsInOrders", conn);
            SQLiteDataReader averageQuantityReader = getAverageQuantity.ExecuteReader();

            while (averageQuantityReader.Read())
            {
                decimal averageQuantity = averageQuantityReader.GetDecimal(0);
                Console.WriteLine($"The average quantity of items a customer orders is: {Math.Round(averageQuantity)}");
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            CheckAdminStockPanel(tableName);
        }//fine don't touch
        public static void FilterInputtedTables(string tableName, string whichFilterMethod)
        {
            string whichOrderMethod = GetOrderMethod();
            int rowChecker = 0;

            Dictionary<string, Dictionary<string, Type>> getDatabaseColumnHeaders = GetColumnHeaders();
            SQLiteCommand getDesiredFilter = new SQLiteCommand($"SELECT * FROM {tableName} ORDER BY {whichFilterMethod} {whichOrderMethod}", conn);
            SQLiteDataReader priceReader = getDesiredFilter.ExecuteReader();
            Console.Clear();

            Console.WriteLine($"Here is the sorted {tableName} table in sorted order of {whichFilterMethod} {whichOrderMethod} \n");

            while (priceReader.Read())
            {
                foreach (var header in getDatabaseColumnHeaders[tableName])
                {
                    string columnHeader = header.Key;

                    if (columnHeader == "Price") { Console.WriteLine($"{columnHeader}: £{priceReader[columnHeader]}"); }
                    else { Console.WriteLine($"{columnHeader}: {priceReader[columnHeader]}"); }

                    rowChecker++;
                    if (rowChecker == getDatabaseColumnHeaders[tableName].Count) { Console.WriteLine(); rowChecker = 0; }
                }
            }

            Console.WriteLine("Items sorted, press any key to continue");
            Console.ReadKey();
            CheckAdminStockPanel(tableName);
        } // completely fine, don't touch
        public static string GetOrderMethod()
        {
            Console.Clear();
            bool hasOrderBeenInputted = false;
            string orderMethod = "";

            while (!hasOrderBeenInputted)
            {
                try
                {
                    Console.WriteLine("Would you like to order the table in ascending or descending order? [asc/desc]");
                    orderMethod = Console.ReadLine().ToLower();

                    if (orderMethod == "asc" || orderMethod == "desc") { hasOrderBeenInputted = true; }
                    else { Console.Clear(); Console.WriteLine("Invalid order property inputted"); }
                }
                catch (Exception ex) when (ex is FormatException) { Console.Clear(); Console.WriteLine("Invalid order property inputted"); }
            }
            return orderMethod;
        } // completely fine, don't touch
        public static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the DBMS, Choose An Option Below:");
            Console.WriteLine();
            Console.WriteLine("1. Admin's Page");
            Console.WriteLine("2. Exit");
            MakeLoginDecision();
        } // completely fine, don't touch
    }
}

