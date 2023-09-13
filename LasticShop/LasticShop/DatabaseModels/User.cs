namespace LasticShop.DatabaseModels
{
    public class User
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
