using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class SurvivorAchievement : Achievement
    {
        public SurvivorAchievement()
        {
            Id = "survivor";
            Title = "Survivor";
            Description = "Win 5 games in a row";
            Icon = "🔥";
        }
        public override bool CanUnlock(GameResult result)
        {
            return result.WinStreak >= 5;
        }
    }
}
