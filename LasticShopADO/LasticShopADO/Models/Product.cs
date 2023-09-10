using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasticShopAdo.Models
{
    internal class Product
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Balance { get; set; }
        public string Valuta { get; set; }
        public string Category { get; set; }
        public List<Review?> Reviews { get; set; }

        public Product()
        {

        }
        public Product(int id, string title)
        {
            Id = id;
            Title = title;
        }
        public Product(int id, string title, decimal price, string currency)
        {
            Id = id;
            Title = title;
            Price = price;
            Valuta = currency;
        }
        public Product(int id, string title, string description, decimal price, int balance, string currency, string criteria)
        {
            Id = id;
            Title = title;
            Description = description;
            Price = price;
            Balance = balance;
            Valuta = currency;
            Category = criteria;
        }
    }
}

