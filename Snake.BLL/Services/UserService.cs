using Snake.DAL.Models;
using Snake.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.BLL.Services
{
    public class UserService
    {
        private UserRepository _repo = new();

        public User Authenticate(string username, string password)
        {
            return _repo.GetOne(username, password);
        }
    }
}
