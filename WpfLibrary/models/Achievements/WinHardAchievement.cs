using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class WinHardAchievement : Achievement
    {
        public WinHardAchievement()
        {
            Id = "hard_win";
            Title = "Fearless";
            Description = "Win on Hard difficulty";
            Icon = "💀";
        }
        public override bool CanUnlock(GameResult result)
        {
            return result.Difficulty == DifficultyLevel.Hard;
        }
    }
}
