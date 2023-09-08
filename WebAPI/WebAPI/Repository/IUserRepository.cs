using WebAPI.Models;

namespace WebAPI.Repository
{
    public interface IUserRepository
    {
        List<UserViewModel> GetCertainNumberOfUsers(int startIndex, int requiredNumberUsers);
        UserViewModel GetById(int id);
        int CreateNewUser(UserViewModel user);
        void Update(UserViewModel user);
        void Delete(int id);
    }
}
