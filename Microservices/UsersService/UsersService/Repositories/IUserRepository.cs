using IoTHubAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUser(string id);
        Task<User> GetUserByEmail(string email);
        Task Delete(User user);
        Task Delete(string userId);
    }
}
