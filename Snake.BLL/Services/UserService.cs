using Snake.DAL.Models;
using Snake.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snake.BLL.Services
{
    public class UserService
    {
        private UserRepository _repo = new();

        public User Authenticate(string email, string password)
        {
            return _repo.GetOne(email, password);
        }

        public bool Exist(string email)
        {
            var account = _repo.GetByEmail(email);
            if (account == null) // Handle null case
            {
                return false;
            }
            return account.Email.Equals(email, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false; 
            }

            try
            {
                string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase);
            }
            catch (RegexMatchTimeoutException)
            {
                return false; 
            }
        }

        public void Register(User user)
        {
            _repo.Add(user);
        }
    }
}
