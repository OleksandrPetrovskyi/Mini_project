namespace LasticShop.Models
{
    public class UserDetails
    {
        public Dictionary<UserGender, string> Gender = new Dictionary<UserGender, string>()
        {
            {UserGender.Male, "Male"},
            {UserGender.Female, "Female"},
            {UserGender.Other, "Other"}
        };
        public Dictionary<UserRole, string> Role = new Dictionary<UserRole, string>()
        {
            {UserRole.User, "User"},
            {UserRole.Admin, "Admin"},
            {UserRole.SuperAdmin, "SuperAdmin"},
        };
    }
    public enum UserGender
    {
        Male,
        Female,
        Other
    }
    public enum UserRole
    {
        User,
        Admin,
        SuperAdmin
    }
}
