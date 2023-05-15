using IoTHubAPI.DatabaseSettings;
using IoTHubAPI.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }
        public async Task<IEnumerable<User>> GetAllUsers() {
            var usersList = await _users.FindAsync(user => true);
            return await usersList.ToListAsync();
        }

        public async Task<User> GetUser(string id) {
            if(id.Length != 24) {
                return null;
            }
            var user = await _users.FindAsync(user => user.Id == id);
            return await user.FirstOrDefaultAsync();
        }

        public async Task<User> Create(User user) {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task Delete(User user) {
            await _users.DeleteOneAsync(userToDelete => userToDelete.Id == user.Id);
        }

        public async Task Delete(string userId) {
            await _users.DeleteOneAsync(userToDelete => userToDelete.Id == userId);
        }

        public async Task<User> GetUserByEmail(string email) {
            var user = await _users.FindAsync(user => user.Email == email);
            return await user.FirstOrDefaultAsync();
        }
    }
}
