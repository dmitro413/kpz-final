using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class FirstWinAchievement : Achievement
    {
        public FirstWinAchievement()
        {
            Id = "first_win";
            Title = "First Blood";
            Description = "Win your first game";
            Icon = "🎉";
        }

        public override bool CanUnlock(GameResult result)
        {
            return result.TotalWins >= 1;
        }
    }
}
