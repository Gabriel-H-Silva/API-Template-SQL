using Microsoft.EntityFrameworkCore;
using ManagerIO.Model.Context;
using System.Security.Cryptography;
using System.Text;
using ManagerApi.Model;


namespace ManagerIO.Repository.Generic
{
    public class UsersRepository
    {
        private MySQLContext _context;

        private DbSet<Users> _dbSet;
        public UsersRepository(MySQLContext context)
        {
            _context = context;
            _dbSet = _context.Set<Users>();
        }
        public Users ValidadeCredentials(Users user)
        {
            try
            {
                var pass = ComputeHash(user.password, SHA256.Create());
                return _context.Users.FirstOrDefault(u =>
                (u.name == user.name) && (u.password == pass));
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public Users ValidadeCredentials(string userName)
        {
            return _context.Users.SingleOrDefault(u => (u.name == userName));
        }

        public bool RevokeToken(string userName)
        {
            var user = _context.Users.SingleOrDefault(u => (u.name == userName));
            if (user is null) return false;
            user.RefreshToken = null;
            _context.SaveChanges();
            return true;
        }
        private object ComputeHash(string? password, HashAlgorithm algorithm)
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

        public Users RefreshUserInfo(Users user)
        {
            if (!_context.Users.Any(u => u.Id.Equals(user.Id))) return null;

            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return result;
        }

        public List<Users> FindAll()
        {
            return _dbSet.ToList();
        }


    }
}
