using MyNutritionist.Data;
using MyNutritionist.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyNutritionist.Utilities
{
    public class RealSubject : ISubject
    {
        private readonly ApplicationDbContext _dbContext;

        public RealSubject()
        {
        }

        public RealSubject(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool Login(string username, string password)
        {
            
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return true;
            }

            return false;
            } catch(Exception ex)
            {
                return false;
            }
            

           
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                // Compute the hash of the provided password
                byte[] passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the hashed password from byte array to string
                string hashedPasswordToCompare = BitConverter.ToString(passwordHash).Replace("-", "");

                // Compare the hashed passwords
                return string.Equals(hashedPassword, hashedPasswordToCompare, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
