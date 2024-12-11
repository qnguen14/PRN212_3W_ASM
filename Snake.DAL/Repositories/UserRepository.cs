using Snake.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.DAL.Repositories
{
    public class UserRepository
    {
        private SnakeGameContext _db;

        public User GetOne(string email, string password)
        {
            _db = new();
            return _db.Users.FirstOrDefault(u => u.Email.Equals(email) && u.Password.Equals(password));
        }
    }
}
