using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasticShopAdo.Models
{
    internal class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Product?> Products { get; set; }

        public User() { }
        public User(int id)
        {
            Id = id;
        }
        public User(int id, string firstName, string secondName, List<Product> products)
        {
            Id = id;
            FirstName = firstName;
            SecondName = secondName;
            Products = products;
        }
        public User(string firstName, string secondName)
        {
            FirstName = firstName;
            SecondName = secondName;
        }
    }
}

