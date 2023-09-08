using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi_Client.Models;
using WebApi_Client.Responses;

namespace WebApi_Client.Repository
{
    internal interface IUserRepository
    {
        Task<ClientResponse<List<User>>> GetAllUsers(int index, int amount);
        Task<ClientResponse<User>> GetUserById(int id);
        Task<string> CreateNewUser(User user);
        Task<string> UpdateUser(User user);
        Task<string> DeleteUser(int id);
    }
}