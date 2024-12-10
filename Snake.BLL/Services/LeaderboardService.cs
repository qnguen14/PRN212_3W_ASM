using Snake.DAL.Models;
using Snake.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.BLL.Services
{
    public class LeaderboardService
    {
        private LeaderboardRepository _repo = new();

        public List<Leaderboard> GetAllScores()
        {
            return _repo.GetAll();
        }

        public void SaveScore(Leaderboard x)
        {
            _repo.Save(x);
        }

        public void UpdateScore(Leaderboard x)
        {
            _repo.Update(x);
        }

        public void DeleteScore(Leaderboard x)
        {
            _repo.Remove(x);
        }
    }
}
