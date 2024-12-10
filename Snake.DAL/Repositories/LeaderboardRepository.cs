using Microsoft.EntityFrameworkCore;
using Snake.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.DAL.Repositories
{
    public class LeaderboardRepository
    {
        private SnakeGameContext _db;

        public List<Leaderboard> GetAll()
        {
            _db = new();
            return _db.Leaderboards.Include("User").ToList();
        }

        public void Save(Leaderboard x) 
        {
            _db = new();
            _db.Leaderboards.Add(x);
            _db.SaveChanges();
        }

        public void Update(Leaderboard x)
        {
            _db = new();
            _db.Leaderboards.Update(x);
            _db.SaveChanges();
        }

        public void Remove(Leaderboard x)
        {
            _db = new();
            _db.Leaderboards.Remove(x);
            _db.SaveChanges();
        }
    }
}
