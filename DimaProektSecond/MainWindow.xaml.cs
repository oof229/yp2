using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using Market;

namespace DimaProektSecond
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Коллекции с продуктами
        private List<Product> products = new List<Product>();
        private List<Product> scannedProducts = new List<Product>();
        private Receipt receipt = new Receipt { StoreName = "My Store" };
        private string lineConnection;

        public MainWindow()
        {
            InitializeComponent();
            Connect("GK216_7\\SQLEXPRESS", "StoreDB"); // Укажите ваш сервер и базу данных
            LoadProducts();
        }

        //Метод подключения к бд
        public void Connect(string servername, string dbname)
        {
            lineConnection = $"Data Source={servername};Initial Catalog={dbname};Integrated Security=True";
        }

        //Загрузка продуктов
        private void LoadProducts()
        {
            products.Clear();
            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                string query = "SELECT Id, Name, Quantity, Barcode, Manufacturer, ExpiryDate, Price, IsOnSale, Discount FROM Products";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Quantity = reader.GetInt32(2),
                        Barcode = reader.GetString(3),
                        Manufacturer = reader.GetString(4),
                        ExpiryDate = reader.GetDateTime(5),
                        Price = reader.GetDecimal(6),
                        IsOnSale = reader.GetBoolean(7),
                        Discount = reader.GetDecimal(8)
                    });
                }
            }
            ProductsList.ItemsSource = products;
        }

        //Добавить продукт
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            string barcode = BarcodeInput.Text;
            var product = products.FirstOrDefault(p => p.Barcode == barcode);
            if (product != null)
            {
                scannedProducts.Add(product);
                ScannedProductsList.ItemsSource = null;
                ScannedProductsList.ItemsSource = scannedProducts;
            }
            else
            {
                MessageBox.Show("Не был найден");
            }
        }

        //удалить продукт из прообитых
        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            string pinCode = PinCodeInput.Password;
            if (pinCode == "1234") 
            {
                if (ScannedProductsList.SelectedItem is Product selectedProduct)
                {
                    scannedProducts.Remove(selectedProduct);
                    ScannedProductsList.ItemsSource = null;
                    ScannedProductsList.ItemsSource = scannedProducts;
                }
            }
            else
            {
                MessageBox.Show("Неверный ПИН-КОД");
            }
        }

        //Печатать чек
        private void PrintReceipt_Click(object sender, RoutedEventArgs e)
        {
            receipt.Products = scannedProducts;
            string receiptText = receipt.GenerateReceipt();
            MessageBox.Show(receiptText);
        }

        //Рассчет скидки
        private void CalculateDiscount_Click(object sender, RoutedEventArgs e)
        {
            receipt.ApplyDiscount();
            ScannedProductsList.ItemsSource = null;
            ScannedProductsList.ItemsSource = scannedProducts;
        }

        //Штрих-код ввод
        private void BarcodeInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (BarcodeInput.Text == "Enter Barcode")
            {
                BarcodeInput.Text = "";
                BarcodeInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void BarcodeInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BarcodeInput.Text))
            {
                BarcodeInput.Text = "Enter Barcode";
                BarcodeInput.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        //Пин-код ввод
        private void PinCodeInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PinCodeInput.Password == "Enter PIN")
            {
                PinCodeInput.Password = "";
                PinCodeInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void PinCodeInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PinCodeInput.Password))
            {
                PinCodeInput.Password = "Enter PIN";
                PinCodeInput.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void PinCodeInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PinCodeInput.Password == "Enter PIN")
            {
                PinCodeInput.Password = "";
                PinCodeInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
    }
}
