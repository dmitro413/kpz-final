using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class Win10GamesAchievement : Achievement
    {
        public Win10GamesAchievement()
        {
            Id = "win_10";
            Title = "Veteran";
            Description = "Win 10 games total";
            Icon = "🏆";
        }

        public override bool CanUnlock(GameResult result)
        {
            return result.TotalWins >= 10;
        }
    }
}
