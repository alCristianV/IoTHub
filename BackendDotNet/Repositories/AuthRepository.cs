using IoTHubAPI.DatabaseSettings;
using IoTHubAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoCollection<User> _users;

        public AuthRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }
        public async Task<User> Login(string email, string password) {
            var user = await _users.FindAsync(x => x.Email == email);
            User userToReturn = user.FirstOrDefaultAsync().Result;
            
            if (userToReturn == null) {
                return null;
            }
            
            if (!VerifyPasswordHash(password, userToReturn.PasswordHash, userToReturn.PasswordSalt)) {
                return null;
            }

            return userToReturn;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password) {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _users.InsertOneAsync(user);

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public async Task<bool> UserExists(string email) {
            var user = await _users.FindAsync(user => user.Email == email);
            if (await user.AnyAsync()) {
                return true;
            }
            return false;
        }
    }
}
