using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class Win50GamesAchievement : Achievement
    {
        public Win50GamesAchievement() 
        {
            Id = "win_50";
            Title = "Legend";
            Description = "Win 50 games total";
            Icon = "👑";
        }
        public override bool CanUnlock(GameResult result)
        {
            return result.TotalWins >= 50;
        }
    }
}
