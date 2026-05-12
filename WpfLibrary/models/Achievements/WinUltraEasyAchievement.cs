using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class WinUltraEasyAchievement : Achievement
    {
        public WinUltraEasyAchievement()
        {
            Id = "ultra_easy_win";
            Title = "Baby Steps";
            Description = "Win on Ultra Easy (it's a start!)";
            Icon = "🧪";
        }
        public override bool CanUnlock(GameResult result)
        {
            return result.Difficulty == DifficultyLevel.UltraEasy;
        }
    }
}
