namespace WebAPI.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Posts> Posts { get; set; }
    }
}