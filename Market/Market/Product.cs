using System;
using System.Collections.Generic;

namespace Market
{

    //Модель БД
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public bool IsOnSale { get; set; }
        public decimal Discount { get; set; }
    }

    //Чек
    public class Receipt
    {
        public string StoreName { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public decimal TotalPrice { get; set; }

        public void CalculateTotalPrice()
        {
            TotalPrice = 0;
            foreach (var product in Products)
            {
                TotalPrice += product.Price * product.Quantity;
            }
        }

        //Подсчет скиждки
        public void ApplyDiscount()
        {
            foreach (var product in Products)
            {
                if (product.IsOnSale)
                {
                    product.Price -= product.Price * (product.Discount / 100);
                }
            }
        }

        //Генерация чека
        public string GenerateReceipt()
        {
            CalculateTotalPrice();
            ApplyDiscount();

            string receipt = $"Store: {StoreName}\n";
            receipt += "Products:\n";
            foreach (var product in Products)
            {
                receipt += $"{product.Name} - {product.Quantity} x {product.Price:C}\n";
            }
            receipt += $"Total Price: {TotalPrice:C}\n";
            return receipt;
        }
    }
}

