using IoTHubAPI.Models;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User?> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<Device> GetDeviceAuthorize(string connectionString);
    }
}
