namespace LasticShop.DatabaseModels
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public string? Positive { get; set; }
        public string? Negative { get; set; }
        public short Rated { get; set; }
        public DateTime Created { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
