namespace LasticShopAdo.Models
{
    internal class Review
    {
        public int UserId { get; set; }
        public string FirsName { get; set; }
        public string SecondName { get; set; }
        public int ProductId { get; set; }
        public int Evalution { get; set; }
        public string? Description { get; set; }

        public Review()
        {

        }
        public Review(int userId, int productId, int evalution, string? Descriprion)
        {
            UserId = userId;
            ProductId = productId;
            Evalution = evalution;
            Description = Descriprion;
        }
    }
}

