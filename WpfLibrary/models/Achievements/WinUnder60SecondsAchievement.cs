using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class WinUnder60SecondsAchievement : Achievement
    {
        public WinUnder60SecondsAchievement()
        {
            Id = "speed_60";
            Title = "Quick Fingers";
            Description = "Win in under 60 seconds";
            Icon = "⏱";
        }
        public override bool CanUnlock(GameResult result)
        {
            return result.TimeSeconds < 60;
        }
    }
}
