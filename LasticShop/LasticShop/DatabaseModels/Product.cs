using System.ComponentModel.DataAnnotations.Schema;

namespace LasticShop.DatabaseModels
{
    public class Product
    {
        public int Id { get; set; }
        [Column("CreatorID")]
        public int UserId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public ICollection<ProductsImages>? IMG { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public User User { get; set; }
    }
}
