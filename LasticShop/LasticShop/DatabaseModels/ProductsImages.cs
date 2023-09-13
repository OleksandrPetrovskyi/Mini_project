using System.ComponentModel.DataAnnotations.Schema;

namespace LasticShop.DatabaseModels
{
    [Table("PrImages")]
    public class ProductsImages
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string IMG { get; set; }
        public Product Product { get; set; }
    }
}
