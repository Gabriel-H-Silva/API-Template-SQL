using Microsoft.EntityFrameworkCore;
using ManagerIO.Model.Context;
using System.Security.Cryptography;
using System.Text;
using ManagerApi.Model;
using ManagerApi.Repository.Interfaces;


namespace ManagerIO.Repository.Generic
{
    public class UsersRepository : IUsersRepository
    {
        private MySQLContext _context;

        private DbSet<Users> _dbSet;
        public UsersRepository(MySQLContext context)
        {
            _context = context;
            _dbSet = _context.Set<Users>();
        }

        // Check Exist User in Database
        public async Task<bool> CheckExistUserById(int id) => await _dbSet.AnyAsync(u => u.Id.Equals(id));

        // Get User by Id in Database
        public async Task<Users?> FindUserById(int id) => await _dbSet.SingleOrDefaultAsync(u => u.Id.Equals(id));

        // Get User by Name in Database
        public async Task<Users?> FindUserByName(string name) => await _dbSet.SingleOrDefaultAsync(u => u.name.Equals(name));

        // Validade Credentials User by UserName in Database
        public async Task<Users?> ValidadeCredentials(string userName) => await _dbSet.SingleOrDefaultAsync(u => (u.name == userName));
        
        // Get All Users In Database
        public async Task<List<Users?>> FindAll() => await _dbSet.ToListAsync();

        // RevokeToken User by userName
        public async Task<bool> RevokeToken(string userName)
        {
            Users? user = await FindUserByName(userName);
            if (user is null) return false;

            user.refreshToken = null;
            await _context.SaveChangesAsync();
            return true;

        }

        // Refresh User Information by User
        public async Task<Users?> RefreshUserInfo(Users user)
        {
            if (!await CheckExistUserById(user.Id)) return null;

            Users? result = await FindUserById(user.Id);
            if (result != null)
            {
                 _context.Entry(result).CurrentValues.SetValues(user);
                 await _context.SaveChangesAsync();
                 return result;
            }

            return result;
        }

        //  Validade Credentials User Password and Name in Database
        public async Task<Users?> ValidadeCredentials(Users user)
        {
            try
            {
                var pass = ComputeHash(user.password, SHA256.Create());
                return await _dbSet.FirstOrDefaultAsync(u => (u.name == user.name) && (u.password == pass));
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        // ComputeHash password
        private object ComputeHash(string password, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            var builder = new StringBuilder();

            foreach (var item in hashedBytes)
            {
                builder.Append(item.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
